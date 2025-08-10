using Api.Abstractions.Application;
using Api.Abstractions.Repositories;
using Api.Database.Entities;
using Shared.Dtos.Volunteer;
using Shared.Enums;
using Shared.Models;

namespace Api.Services.Application
{
    public class VolunteerRequestService : IVolunteerRequestService
    {
        private readonly IVolunteerRequestRepository _volunteerRequestRepository;

        public VolunteerRequestService(IVolunteerRequestRepository volunteerRequestRepository)
        {
            _volunteerRequestRepository = volunteerRequestRepository;
        }

        public async Task<List<VolunteerRequestDto>> GetAllByVolunteerIDAsync(int volunteerId)
        {
            var requests = await _volunteerRequestRepository.GetRequestsByVolunteerID(volunteerId);
            var dtos = new List<VolunteerRequestDto>();

            foreach (var r in requests)
            {
                dtos.Add(await MapToDtoAsync(r));
            }

            return dtos;
        }

        // ===== VALIDACIÓN PARA NUEVA SOLICITUD =====
        public async Task<Result> CanCreateNewRequestAsync(int volunteerId)
        {
            try
            {
                var existingRequests = await _volunteerRequestRepository.GetRequestsByVolunteerID(volunteerId);

                // Si no hay solicitudes, puede crear nueva
                if (existingRequests == null || !existingRequests.Any())
                    return Result.Success();

                // REQUERIMIENTO: No puede haber más de una solicitud activa por voluntario
                // PERO puede crear nueva si ya cumplió las horas de la anterior
                var activeRequest = existingRequests.FirstOrDefault(r =>
                    r.State == VolunteerState.Pending ||
                    (r.State == VolunteerState.Approved));

                if (activeRequest != null)
                {
                    // Si tiene solicitud aprobada, verificar si ya cumplió las horas
                    if (activeRequest.State == VolunteerState.Approved)
                    {
                        var totalWorkedHours = await _volunteerRequestRepository.GetTotalApprovedHoursAsync(activeRequest.Id);
                        if (totalWorkedHours < activeRequest.Hours)
                        {
                            return Result.Failure("Ya tienes una solicitud aprobada con horas pendientes de completar. Completa las horas restantes antes de crear una nueva solicitud.");
                        }

                        // OPCIONAL: Cerrar automáticamente la solicitud completada
                        if (totalWorkedHours >= activeRequest.Hours && activeRequest.State != VolunteerState.Closed)
                        {
                            activeRequest.State = VolunteerState.Closed;
                            await _volunteerRequestRepository.UpdateRequestAsync(activeRequest);
                        }

                        // Si ya cumplió las horas, puede crear nueva solicitud
                        return Result.Success();
                    }
                    else if (activeRequest.State == VolunteerState.Pending)
                    {
                        return Result.Failure("Ya tienes una solicitud pendiente de aprobación. Espera a que sea aprobada antes de crear una nueva.");
                    }
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error al validar solicitud: {ex.Message}");
            }
        }

        // ===== MÉTODOS CREATE ACTUALIZADOS =====

        // Nuevo método principal que usa CreateVolunteerRequestDto
        public async Task<Result> CreateAsync(CreateVolunteerRequestDto requestDto, int volunteerId)
        {
            // Validación de negocio (doble verificación por seguridad)
            var canCreateResult = await CanCreateNewRequestAsync(volunteerId);
            if (canCreateResult.IsFailure)
                return canCreateResult;

            var request = new VolunteerRequest
            {
                VolunteerId = volunteerId,
                Institution = requestDto.Institution,
                Profession = requestDto.Profession,
                Description = requestDto.Description,
                Hours = requestDto.Hours,
                State = VolunteerState.Pending
            };

            await _volunteerRequestRepository.CreateRequest(request);
            return Result.Success();
        }

        // Método legacy - mantenido por compatibilidad si se usa en otras partes
        public async Task<Result> CreateAsync(VolunteerRequestDto requestDto)
        {
            // Usar el nuevo método de validación
            var canCreateResult = await CanCreateNewRequestAsync(requestDto.VolunteerId);
            if (canCreateResult.IsFailure)
                return canCreateResult;

            var request = new VolunteerRequest
            {
                VolunteerId = requestDto.VolunteerId,
                Institution = requestDto.Institution,
                Profession = requestDto.Profession,
                Description = requestDto.Description,
                Hours = requestDto.Hours,
                State = VolunteerState.Pending
            };

            await _volunteerRequestRepository.CreateRequest(request);
            return Result.Success();
        }

        // ===== MÉTODOS DE ADMINISTRACIÓN =====

        public async Task<List<VolunteerRequestDto>> GetAllRequestsAsync()
        {
            var requests = await _volunteerRequestRepository.GetAllRequestsAsync();
            var list = new List<VolunteerRequestDto>();
            foreach (var r in requests)
                list.Add(await MapToDtoAsync(r));
            return list;
        }

        public async Task<List<VolunteerRequestDto>> GetRequestsByStateAsync(VolunteerState state)
        {
            var requests = await _volunteerRequestRepository.GetRequestsByStateAsync(state);
            var list = new List<VolunteerRequestDto>();
            foreach (var r in requests)
                list.Add(await MapToDtoAsync(r));
            return list;
        }

        public async Task<Result<VolunteerRequestDto>> GetRequestByIdAsync(int requestId)
        {
            var request = await _volunteerRequestRepository.GetRequestByIdAsync(requestId);
            if (request == null)
                return Result<VolunteerRequestDto>.Failure("Solicitud no encontrada");

            return Result<VolunteerRequestDto>.Success(await MapToDtoAsync(request));
        }

        public async Task<Result> ApproveRequestAsync(int requestId, int approverId)
        {
            var request = await _volunteerRequestRepository.GetRequestByIdAsync(requestId);
            if (request == null)
                return Result.Failure("Solicitud no encontrada");
            if (request.State != VolunteerState.Pending)
                return Result.Failure("Solo se pueden aprobar solicitudes pendientes");

            await _volunteerRequestRepository.ApproveRequestAsync(requestId, approverId);
            return Result.Success();
        }

        public async Task<Result> RejectRequestAsync(int requestId, int approverId, string reason)
        {
            var request = await _volunteerRequestRepository.GetRequestByIdAsync(requestId);
            if (request == null)
                return Result.Failure("Solicitud no encontrada");
            if (request.State != VolunteerState.Pending)
                return Result.Failure("Solo se pueden rechazar solicitudes pendientes");
            if (string.IsNullOrWhiteSpace(reason))
                return Result.Failure("Debe proporcionar una razón para el rechazo");

            await _volunteerRequestRepository.RejectRequestAsync(requestId, approverId, reason);
            return Result.Success();
        }

        // ===== GESTIÓN DE HORAS =====

        public async Task<Result> CreateVolunteerHoursAsync(CreateVolunteerHoursDto dto)
        {
            var request = await _volunteerRequestRepository.GetRequestByIdAsync(dto.VolunteerRequestId);
            if (request == null)
                return Result.Failure("Solicitud de voluntariado no encontrada");
            if (request.State != VolunteerState.Approved)
                return Result.Failure("Solo se pueden registrar horas para solicitudes aprobadas");

            // ✅ REQUERIMIENTO 1: VALIDAR HORAS PROPUESTAS VS RESTANTES
            var totalApprovedHours = await _volunteerRequestRepository.GetTotalApprovedHoursAsync(dto.VolunteerRequestId);
            var totalHours = CalculateHours(dto.StartTime, dto.EndTime);
            var remainingHours = request.Hours - totalApprovedHours;

            if (totalHours > remainingHours)
            {
                return Result.Failure($"No puedes registrar {totalHours} horas. Solo quedan {remainingHours} horas disponibles de las {request.Hours} horas propuestas.");
            }

            var validationResult = await ValidateHoursAsync(dto);
            if (validationResult.IsFailure)
                return validationResult;

            // ✅ REQUERIMIENTO 3: PERMITIR RE-REGISTRO DESPUÉS DE RECHAZO
            var existingHours = await _volunteerRequestRepository.GetHoursForDateAsync(dto.VolunteerRequestId, dto.Date);
            if (existingHours != null)
            {
                if (existingHours.State == VolunteerState.Approved)
                    return Result.Failure("Ya existe un registro de horas aprobado para esta fecha");

                if (existingHours.State == VolunteerState.Pending)
                    return Result.Failure("Ya existe un registro de horas pendiente de aprobación para esta fecha");

                // Si está rechazada, eliminar la anterior para permitir nueva
                if (existingHours.State == VolunteerState.Rejected)
                {
                    await _volunteerRequestRepository.DeleteVolunteerHoursAsync(existingHours.Id);
                }
            }

            var volunteerHours = new VolunteerHours
            {
                VolunteerRequestId = dto.VolunteerRequestId,
                Date = dto.Date,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                TotalHours = totalHours,
                ActivitiesDescription = dto.ActivitiesDescription,
                Notes = dto.Notes,
                State = VolunteerState.Pending
            };

            await _volunteerRequestRepository.AddVolunteerHoursAsync(volunteerHours);
            return Result.Success();
        }

        public async Task<Result<List<VolunteerHoursDto>>> GetHoursByRequestIdAsync(int requestId)
        {
            var hours = await _volunteerRequestRepository.GetHoursByRequestIdAsync(requestId);
            var dtos = hours.Select(h => MapHoursToDto(h)).ToList();
            return Result<List<VolunteerHoursDto>>.Success(dtos);
        }

        public async Task<Result<List<VolunteerHoursDto>>> GetHoursByDateRangeAsync(int requestId, DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                return Result<List<VolunteerHoursDto>>.Failure("La fecha de inicio no puede ser mayor a la fecha de fin");

            var hours = await _volunteerRequestRepository.GetHoursByDateRangeAsync(requestId, startDate, endDate);
            var dtos = hours.Select(h => MapHoursToDto(h)).ToList();
            return Result<List<VolunteerHoursDto>>.Success(dtos);
        }

        public async Task<Result> UpdateVolunteerHoursAsync(int hoursId, CreateVolunteerHoursDto dto)
        {
            var existingHours = await _volunteerRequestRepository.GetVolunteerHoursAsync(hoursId);
            if (existingHours == null)
                return Result.Failure("Registro de horas no encontrado");
            if (existingHours.State == VolunteerState.Approved)
                return Result.Failure("No se pueden modificar horas ya aprobadas");

            // ✅ REQUERIMIENTO 1: VALIDAR HORAS PROPUESTAS VS RESTANTES AL ACTUALIZAR
            var request = await _volunteerRequestRepository.GetRequestByIdAsync(dto.VolunteerRequestId);
            if (request == null)
                return Result.Failure("Solicitud de voluntariado no encontrada");

            var totalApprovedHours = await _volunteerRequestRepository.GetTotalApprovedHoursAsync(dto.VolunteerRequestId);
            var totalHours = CalculateHours(dto.StartTime, dto.EndTime);
            var remainingHours = request.Hours - totalApprovedHours;

            if (totalHours > remainingHours)
            {
                return Result.Failure($"No puedes registrar {totalHours} horas. Solo quedan {remainingHours} horas disponibles de las {request.Hours} horas propuestas.");
            }

            var validationResult = await ValidateHoursAsync(dto);
            if (validationResult.IsFailure)
                return validationResult;

            // ✅ REQUERIMIENTO 3: MANEJAR CONFLICTOS DE FECHA CON RECHAZOS
            if (existingHours.Date.Date != dto.Date.Date)
            {
                var conflicting = await _volunteerRequestRepository.GetHoursForDateAsync(dto.VolunteerRequestId, dto.Date);
                if (conflicting != null && conflicting.Id != hoursId)
                {
                    if (conflicting.State == VolunteerState.Approved || conflicting.State == VolunteerState.Pending)
                        return Result.Failure("Ya existe un registro de horas para esta fecha");

                    // Si está rechazada, eliminar la conflictiva
                    if (conflicting.State == VolunteerState.Rejected)
                    {
                        await _volunteerRequestRepository.DeleteVolunteerHoursAsync(conflicting.Id);
                    }
                }
            }

            existingHours.Date = dto.Date;
            existingHours.StartTime = dto.StartTime;
            existingHours.EndTime = dto.EndTime;
            existingHours.TotalHours = totalHours;
            existingHours.ActivitiesDescription = dto.ActivitiesDescription;
            existingHours.Notes = dto.Notes;
            existingHours.State = VolunteerState.Pending;

            await _volunteerRequestRepository.UpdateVolunteerHoursAsync(existingHours);
            return Result.Success();
        }

        public async Task<Result> DeleteVolunteerHoursAsync(int hoursId)
        {
            var hours = await _volunteerRequestRepository.GetVolunteerHoursAsync(hoursId);
            if (hours == null)
                return Result.Failure("Registro de horas no encontrado");
            if (hours.State == VolunteerState.Approved)
                return Result.Failure("No se pueden eliminar horas ya aprobadas");

            await _volunteerRequestRepository.DeleteVolunteerHoursAsync(hoursId);
            return Result.Success();
        }

        // ===== APROBACIÓN DE HORAS =====

        public async Task<Result> ApproveHoursAsync(ApproveRejectHoursDto dto)
        {
            var hours = await _volunteerRequestRepository.GetVolunteerHoursAsync(dto.HoursId);
            if (hours == null)
                return Result.Failure("Registro de horas no encontrado");
            if (hours.State != VolunteerState.Pending)
                return Result.Failure("Solo se pueden aprobar horas pendientes");

            await _volunteerRequestRepository.ApproveHoursAsync(dto.HoursId, dto.ApproverId);

            // OPCIONAL: Verificar si se debe cerrar la solicitud automáticamente
            await CheckAndCloseRequestIfComplete(hours.VolunteerRequestId);

            return Result.Success();
        }

        public async Task<Result> RejectHoursAsync(ApproveRejectHoursDto dto)
        {
            var hours = await _volunteerRequestRepository.GetVolunteerHoursAsync(dto.HoursId);
            if (hours == null)
                return Result.Failure("Registro de horas no encontrado");
            if (hours.State != VolunteerState.Pending)
                return Result.Failure("Solo se pueden rechazar horas pendientes");
            if (string.IsNullOrWhiteSpace(dto.RejectionReason))
                return Result.Failure("Debe proporcionar una razón para el rechazo");

            await _volunteerRequestRepository.RejectHoursAsync(dto.HoursId, dto.ApproverId, dto.RejectionReason);
            return Result.Success();
        }

        public async Task<List<VolunteerHoursDto>> GetPendingHoursAsync()
        {
            var hours = await _volunteerRequestRepository.GetPendingHoursAsync();
            return hours.Select(h => MapHoursToDto(h)).ToList();
        }

        // ===== VALIDACIONES =====

        public async Task<Result> ValidateHoursAsync(CreateVolunteerHoursDto dto)
        {
            var errors = new List<string>();

            if (dto.Date.Date > DateTime.Now.Date)
                errors.Add("No se pueden registrar horas para fechas futuras");

            if (dto.Date.Date < DateTime.Now.Date.AddDays(-30))
                errors.Add("No se pueden registrar horas para fechas anteriores a 30 días");

            if (dto.StartTime >= dto.EndTime)
                errors.Add("La hora de inicio debe ser menor a la hora de fin");

            var totalHours = CalculateHours(dto.StartTime, dto.EndTime);

            if (totalHours > 8)
                errors.Add("No se pueden registrar más de 8 horas por día");

            if (totalHours < 1)
                errors.Add("Debe registrar al menos 1 hora de trabajo");

            // REMOVIDO: Restricción de horario según requerimientos ("no tiene restriccion de horario de trabajo")

            if (errors.Any())
                return Result.Failure(errors);
            return Result.Success();
        }

        public async Task<bool> CanRegisterHoursAsync(int requestId, DateTime date)
        {
            var request = await _volunteerRequestRepository.GetRequestByIdAsync(requestId);
            if (request?.State != VolunteerState.Approved)
                return false;

            var hasHoursForDate = await _volunteerRequestRepository.HasHoursForDateAsync(requestId, date);
            return !hasHoursForDate;
        }

        // ===== NUEVOS MÉTODOS REQUERIDOS =====

        // ✅ MÉTODO AGREGADO PARA SOLUCIONAR "Registro de horas no encontrado"
        public async Task<Result<VolunteerHoursDto>> GetVolunteerHoursByIdAsync(int hoursId)
        {
            var volunteerHours = await _volunteerRequestRepository.GetVolunteerHoursAsync(hoursId);
            if (volunteerHours == null)
                return Result<VolunteerHoursDto>.Failure("Registro de horas no encontrado");

            var dto = MapHoursToDto(volunteerHours);
            return Result<VolunteerHoursDto>.Success(dto);
        }

        // ===== MÉTODOS AUXILIARES =====

        private async Task CheckAndCloseRequestIfComplete(int requestId)
        {
            try
            {
                var request = await _volunteerRequestRepository.GetRequestByIdAsync(requestId);
                if (request?.State == VolunteerState.Approved)
                {
                    var totalWorkedHours = await _volunteerRequestRepository.GetTotalApprovedHoursAsync(requestId);
                    if (totalWorkedHours >= request.Hours)
                    {
                        // Cerrar la solicitud automáticamente
                        request.State = VolunteerState.Closed;
                        await _volunteerRequestRepository.UpdateRequestAsync(request);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log del error pero no fallar la operación principal
                Console.WriteLine($"Error al verificar cierre automático de solicitud {requestId}: {ex.Message}");
            }
        }

        private async Task<VolunteerRequestDto> MapToDtoAsync(VolunteerRequest request)
        {
            var workedHours = await _volunteerRequestRepository.GetTotalApprovedHoursAsync(request.Id);
            return new VolunteerRequestDto
            {
                Id = request.Id,
                VolunteerId = request.VolunteerId,
                VolunteerName = $"{request.Volunteer?.Nombre} {request.Volunteer?.Apellidos}",
                ApproverId = request.ApproverId,
                ApproverName = request.Approver != null ? $"{request.Approver?.Nombre} {request.Approver?.Apellidos}" : null,
                CreatedAt = request.CreatedAt,
                ApprovedAt = request.ApprovedAt,
                State = request.State,
                Institution = request.Institution,
                Profession = request.Profession,
                Description = request.Description,
                Hours = request.Hours,
                RejectionReason = request.RejectionReason,
                HoursWorked = workedHours
            };
        }

        private static VolunteerHoursDto MapHoursToDto(VolunteerHours hours)
        {
            return new VolunteerHoursDto
            {
                Id = hours.Id,
                VolunteerRequestId = hours.VolunteerRequestId,
                ApproverId = hours.ApproverId,
                VolunteerName = hours.VolunteerRequest?.Volunteer != null
                    ? $"{hours.VolunteerRequest.Volunteer.Nombre} {hours.VolunteerRequest.Volunteer.Apellidos}"
                    : "N/A",
                Date = hours.Date,
                StartTime = hours.StartTime,
                EndTime = hours.EndTime,
                TotalHours = hours.TotalHours,
                ActivitiesDescription = hours.ActivitiesDescription,
                Notes = hours.Notes,
                State = hours.State,
                CreatedAt = hours.CreatedAt,
                ApprovedAt = hours.ApprovedAt,
                RejectionReason = hours.RejectionReason,
            };
        }

        private static decimal CalculateHours(TimeSpan startTime, TimeSpan endTime)
        {
            return (decimal)(endTime - startTime).TotalHours;
        }
    }
}
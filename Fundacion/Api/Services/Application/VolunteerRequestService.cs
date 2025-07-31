using Api.Abstractions.Application;
using Api.Abstractions.Repositories;
using Api.Database.Entities;
using Api.Database.Repositories;
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
            this._volunteerRequestRepository = volunteerRequestRepository;
        }
        public async Task<List<VolunteerRequestDto>> GetAllByVolunteerIDAsync(int VolunteerId)
        {
            var requests = await _volunteerRequestRepository.GetRequestsByVolunteerID(VolunteerId);
            return requests.Select(r => new VolunteerRequestDto
            {
                Id = r.Id,
                VolunteerId = r.VolunteerId,
                VolunteerName = $"{r.Volunteer?.Nombre} {r.Volunteer?.Apellidos}",
                ApproverId = r.ApproverId,
                ApproverName = r.Approver != null ? $"{r.Approver?.Nombre} {r.Approver?.Apellidos}" : null,
                CreatedAt = r.CreatedAt,
                State = r.State,
                Institution = r.Institution,
                Profession = r.Profession,
                Description = r.Description,
                Hours = r.Hours
            }).ToList();

        }
        public async Task<Result> CreateAsync(VolunteerRequestDto requestDto)
        {
            var ativeRequest = await _volunteerRequestRepository.GetActiveRequest(requestDto.VolunteerId);
            if (ativeRequest != null)
            {
                return Result.Failure("Usted ya tiene una solicitud en proceso");
            }
            var request = new VolunteerRequest
            {
                VolunteerId = requestDto.VolunteerId,
                Institution = requestDto.Institution,
                Profession = requestDto.Profession,
                Description = requestDto.Description,
                Hours = requestDto.Hours,
                State = Shared.Enums.VolunteerState.Pending
            };
            await _volunteerRequestRepository.CreateRequest(request);
            return Result.Success();
        }
        // ===== NUEVOS MÉTODOS PARA ADMINISTRACIÓN =====
        public async Task<List<VolunteerRequestDto>> GetAllRequestsAsync()
        {
            var requests = await _volunteerRequestRepository.GetAllRequestsAsync();
            return requests.Select(r => MapToDto(r)).ToList();
        }

        public async Task<List<VolunteerRequestDto>> GetRequestsByStateAsync(VolunteerState state)
        {
            var requests = await _volunteerRequestRepository.GetRequestsByStateAsync(state);
            return requests.Select(r => MapToDto(r)).ToList();
        }

        public async Task<Result<VolunteerRequestDto>> GetRequestByIdAsync(int requestId)
        {
            var request = await _volunteerRequestRepository.GetRequestByIdAsync(requestId);
            if (request == null)
            {
                return Result<VolunteerRequestDto>.Failure("Solicitud no encontrada");
            }

            return Result<VolunteerRequestDto>.Success(MapToDto(request));
        }

        public async Task<Result> ApproveRequestAsync(int requestId, int approverId)
        {
            var request = await _volunteerRequestRepository.GetRequestByIdAsync(requestId);
            if (request == null)
            {
                return Result.Failure("Solicitud no encontrada");
            }

            if (request.State != VolunteerState.Pending)
            {
                return Result.Failure("Solo se pueden aprobar solicitudes pendientes");
            }

            await _volunteerRequestRepository.ApproveRequestAsync(requestId, approverId);
            return Result.Success();
        }

        public async Task<Result> RejectRequestAsync(int requestId, int approverId, string reason)
        {
            var request = await _volunteerRequestRepository.GetRequestByIdAsync(requestId);
            if (request == null)
            {
                return Result.Failure("Solicitud no encontrada");
            }

            if (request.State != VolunteerState.Pending)
            {
                return Result.Failure("Solo se pueden rechazar solicitudes pendientes");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return Result.Failure("Debe proporcionar una razón para el rechazo");
            }

            await _volunteerRequestRepository.RejectRequestAsync(requestId, approverId, reason);
            return Result.Success();
        }

        // ===== GESTIÓN DE HORAS =====
        public async Task<Result> CreateVolunteerHoursAsync(CreateVolunteerHoursDto dto)
        {
            // Validar que la solicitud existe y está aprobada
            var request = await _volunteerRequestRepository.GetRequestByIdAsync(dto.VolunteerRequestId);
            if (request == null)
            {
                return Result.Failure("Solicitud de voluntariado no encontrada");
            }

            if (request.State != VolunteerState.Approved)
            {
                return Result.Failure("Solo se pueden registrar horas para solicitudes aprobadas");
            }

            // Validar las horas
            var validationResult = await ValidateHoursAsync(dto);
            if (validationResult.IsFailure)
            {
                return validationResult;
            }

            // Verificar que no haya registro para esa fecha
            var existingHours = await _volunteerRequestRepository.GetHoursForDateAsync(dto.VolunteerRequestId, dto.Date);
            if (existingHours != null)
            {
                return Result.Failure("Ya existe un registro de horas para esta fecha");
            }

            // Calcular total de horas
            var totalHours = CalculateHours(dto.StartTime, dto.EndTime);

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
            {
                return Result<List<VolunteerHoursDto>>.Failure("La fecha de inicio no puede ser mayor a la fecha de fin");
            }

            var hours = await _volunteerRequestRepository.GetHoursByDateRangeAsync(requestId, startDate, endDate);
            var dtos = hours.Select(h => MapHoursToDto(h)).ToList();
            return Result<List<VolunteerHoursDto>>.Success(dtos);
        }

        public async Task<Result> UpdateVolunteerHoursAsync(int hoursId, CreateVolunteerHoursDto dto)
        {
            var existingHours = await _volunteerRequestRepository.GetVolunteerHoursAsync(hoursId);
            if (existingHours == null)
            {
                return Result.Failure("Registro de horas no encontrado");
            }

            if (existingHours.State == VolunteerState.Approved)
            {
                return Result.Failure("No se pueden modificar horas ya aprobadas");
            }

            // Validar las nuevas horas
            var validationResult = await ValidateHoursAsync(dto);
            if (validationResult.IsFailure)
            {
                return validationResult;
            }

            // Verificar que no haya otro registro para la nueva fecha (si cambió)
            if (existingHours.Date.Date != dto.Date.Date)
            {
                var conflictingHours = await _volunteerRequestRepository.GetHoursForDateAsync(dto.VolunteerRequestId, dto.Date);
                if (conflictingHours != null && conflictingHours.Id != hoursId)
                {
                    return Result.Failure("Ya existe un registro de horas para esta fecha");
                }
            }

            // Actualizar
            var totalHours = CalculateHours(dto.StartTime, dto.EndTime);
            existingHours.Date = dto.Date;
            existingHours.StartTime = dto.StartTime;
            existingHours.EndTime = dto.EndTime;
            existingHours.TotalHours = totalHours;
            existingHours.ActivitiesDescription = dto.ActivitiesDescription;
            existingHours.Notes = dto.Notes;
            existingHours.State = VolunteerState.Pending; // Requiere nueva aprobación

            await _volunteerRequestRepository.UpdateVolunteerHoursAsync(existingHours);
            return Result.Success();
        }

        public async Task<Result> DeleteVolunteerHoursAsync(int hoursId)
        {
            var hours = await _volunteerRequestRepository.GetVolunteerHoursAsync(hoursId);
            if (hours == null)
            {
                return Result.Failure("Registro de horas no encontrado");
            }

            if (hours.State == VolunteerState.Approved)
            {
                return Result.Failure("No se pueden eliminar horas ya aprobadas");
            }

            await _volunteerRequestRepository.DeleteVolunteerHoursAsync(hoursId);
            return Result.Success();
        }

        // ===== APROBACIÓN DE HORAS =====
        public async Task<Result> ApproveHoursAsync(ApproveRejectHoursDto dto)
        {
            var hours = await _volunteerRequestRepository.GetVolunteerHoursAsync(dto.HoursId);
            if (hours == null)
            {
                return Result.Failure("Registro de horas no encontrado");
            }

            if (hours.State != VolunteerState.Pending)
            {
                return Result.Failure("Solo se pueden aprobar horas pendientes");
            }

            await _volunteerRequestRepository.ApproveHoursAsync(dto.HoursId, dto.ApproverId);
            return Result.Success();
        }

        public async Task<Result> RejectHoursAsync(ApproveRejectHoursDto dto)
        {
            var hours = await _volunteerRequestRepository.GetVolunteerHoursAsync(dto.HoursId);
            if (hours == null)
            {
                return Result.Failure("Registro de horas no encontrado");
            }

            if (hours.State != VolunteerState.Pending)
            {
                return Result.Failure("Solo se pueden rechazar horas pendientes");
            }

            if (string.IsNullOrWhiteSpace(dto.RejectionReason))
            {
                return Result.Failure("Debe proporcionar una razón para el rechazo");
            }

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

            // Validar fecha no sea futura
            if (dto.Date.Date > DateTime.Now.Date)
            {
                errors.Add("No se pueden registrar horas para fechas futuras");
            }

            // Validar fecha no sea muy antigua (ej: más de 1 mes)
            if (dto.Date.Date < DateTime.Now.Date.AddDays(-30))
            {
                errors.Add("No se pueden registrar horas para fechas anteriores a 30 días");
            }

            // Validar horarios
            if (dto.StartTime >= dto.EndTime)
            {
                errors.Add("La hora de inicio debe ser menor a la hora de fin");
            }

            // Calcular total de horas
            var totalHours = CalculateHours(dto.StartTime, dto.EndTime);

            // Validar máximo 8 horas por día
            if (totalHours > 8)
            {
                errors.Add("No se pueden registrar más de 8 horas por día");
            }

            // Validar mínimo 1 hora
            if (totalHours < 1)
            {
                errors.Add("Debe registrar al menos 1 hora de trabajo");
            }

            // Validar horarios laborales razonables (ej: 6:00 AM - 10:00 PM)
            if (dto.StartTime < TimeSpan.FromHours(6) || dto.EndTime > TimeSpan.FromHours(22))
            {
                errors.Add("Los horarios deben estar entre 6:00 AM y 10:00 PM");
            }

            if (errors.Any())
            {
                return Result.Failure(errors);
            }

            return Result.Success();
        }

        public async Task<bool> CanRegisterHoursAsync(int requestId, DateTime date)
        {
            var request = await _volunteerRequestRepository.GetRequestByIdAsync(requestId);
            if (request?.State != VolunteerState.Approved)
            {
                return false;
            }

            var hasHoursForDate = await _volunteerRequestRepository.HasHoursForDateAsync(requestId, date);
            return !hasHoursForDate;
        }

        // ===== MÉTODOS AUXILIARES =====
        private static VolunteerRequestDto MapToDto(VolunteerRequest request)
        {
            return new VolunteerRequestDto
            {
                Id = request.Id,
                VolunteerId = request.VolunteerId,
                VolunteerName = $"{request.Volunteer?.Nombre} {request.Volunteer?.Apellidos}",
                ApproverId = request.ApproverId,
                ApproverName = request.Approver != null ? $"{request.Approver?.Nombre} {request.Approver?.Apellidos}" : null,
                CreatedAt = request.CreatedAt,
                State = request.State,
                Institution = request.Institution,
                Profession = request.Profession,
                Description = request.Description,
                Hours = request.Hours
            };
        }

        private static VolunteerHoursDto MapHoursToDto(VolunteerHours hours)
        {
            return new VolunteerHoursDto
            {
                Id = hours.Id,
                VolunteerRequestId = hours.VolunteerRequestId,
                ApproverId = hours.ApproverId,                
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
            var difference = endTime - startTime;
            return (decimal)difference.TotalHours;
        }
    }
}
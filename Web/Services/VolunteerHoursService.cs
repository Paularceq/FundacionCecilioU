using Shared.Dtos.Volunteer;
using Shared.Enums;
using Shared.Models;
using Web.Http;
using Web.Models.Volunteer;

namespace Web.Services
{
    public class VolunteerHoursService
    {
        private readonly ApiClient _apiClient;

        public VolunteerHoursService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        // ===== GESTIÓN BÁSICA DE HORAS =====
        public async Task<Result> CreateVolunteerHoursAsync(CreateVolunteerHoursViewModel model)
        {
            try
            {
                // ✅ REQUERIMIENTO 1: Validar horas restantes ANTES de enviar a API
                var validationResult = await ValidateHoursWithRemainingHoursAsync(model);
                if (validationResult.IsFailure)
                {
                    return validationResult;
                }

                // ✅ REQUERIMIENTO 3: Verificar registro existente para la fecha
                var existingHoursResult = await CheckExistingHoursForDateAsync(model.VolunteerRequestId, model.Date);
                if (existingHoursResult.IsFailure)
                {
                    return existingHoursResult;
                }

                var dto = new CreateVolunteerHoursDto
                {
                    VolunteerRequestId = model.VolunteerRequestId,
                    Date = model.Date,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    ActivitiesDescription = model.ActivitiesDescription,
                    Notes = model.Notes
                };

                return await _apiClient.PostAsync("volunteerhours", dto);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error al crear registro de horas: {ex.Message}");
            }
        }

        // ✅ MÉTODO LEGACY MANTENIDO PARA COMPATIBILIDAD
        public async Task<Result> CreateVolunteerHoursAsync(AddHoursViewModel model)
        {
            try
            {
                // Validaciones del lado cliente antes de enviar
                var validationResult = await ValidateHoursClientSideAsync(model);
                if (validationResult.IsFailure)
                {
                    return validationResult;
                }

                var dto = new CreateVolunteerHoursDto
                {
                    VolunteerRequestId = model.RequestId,
                    Date = model.Date,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    ActivitiesDescription = model.ActivitiesDescription,
                    Notes = model.Notes
                };

                return await _apiClient.PostAsync("volunteerhours", dto);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error al crear registro de horas: {ex.Message}");
            }
        }

        public async Task<Result<List<VolunteerHoursDto>>> GetHoursByRequestIdAsync(int requestId)
        {
            try
            {
                var result = await _apiClient.GetAsync<List<VolunteerHoursDto>>($"volunteerhours/request/{requestId}");

                if (result.IsSuccess && result.Value == null)
                {
                    return Result<List<VolunteerHoursDto>>.Success(new List<VolunteerHoursDto>());
                }

                return result;
            }
            catch (Exception ex)
            {
                return Result<List<VolunteerHoursDto>>.Failure($"Error al obtener horas: {ex.Message}");
            }
        }

        public async Task<Result<List<VolunteerHoursDto>>> GetHoursByDateRangeAsync(int requestId, DateTime startDate, DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    return Result<List<VolunteerHoursDto>>.Failure("La fecha de inicio no puede ser mayor a la fecha de fin");
                }

                var url = $"volunteerhours/request/{requestId}/date-range?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
                var result = await _apiClient.GetAsync<List<VolunteerHoursDto>>(url);

                if (result.IsSuccess && result.Value == null)
                {
                    return Result<List<VolunteerHoursDto>>.Success(new List<VolunteerHoursDto>());
                }

                return result;
            }
            catch (Exception ex)
            {
                return Result<List<VolunteerHoursDto>>.Failure($"Error al obtener horas por rango de fechas: {ex.Message}");
            }
        }

        public async Task<Result<VolunteerHoursDto>> GetHoursByIdAsync(int hoursId)
        {
            try
            {
                return await _apiClient.GetAsync<VolunteerHoursDto>($"volunteerhours/{hoursId}");
            }
            catch (Exception ex)
            {
                return Result<VolunteerHoursDto>.Failure($"Error al obtener registro de horas: {ex.Message}");
            }
        }

        public async Task<Result> UpdateVolunteerHoursAsync(int hoursId, CreateVolunteerHoursViewModel model)
        {
            try
            {
                // ✅ REQUERIMIENTO 2: Verificar que el registro existe antes de actualizar
                var existingHoursResult = await GetHoursByIdAsync(hoursId);
                if (existingHoursResult.IsFailure)
                {
                    return Result.Failure("Registro de horas no encontrado");
                }

                // ✅ REQUERIMIENTO 1: Validar horas restantes
                var validationResult = await ValidateHoursWithRemainingHoursAsync(model);
                if (validationResult.IsFailure)
                {
                    return validationResult;
                }

                var dto = new CreateVolunteerHoursDto
                {
                    VolunteerRequestId = model.VolunteerRequestId,
                    Date = model.Date,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    ActivitiesDescription = model.ActivitiesDescription,
                    Notes = model.Notes
                };

                return await _apiClient.PutAsync($"volunteerhours/{hoursId}", dto);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error al actualizar registro de horas: {ex.Message}");
            }
        }

        // ✅ MÉTODO LEGACY MANTENIDO PARA COMPATIBILIDAD
        public async Task<Result> UpdateVolunteerHoursAsync(int hoursId, AddHoursViewModel model)
        {
            try
            {
                // Validaciones del lado cliente
                var validationResult = await ValidateHoursClientSideAsync(model);
                if (validationResult.IsFailure)
                {
                    return validationResult;
                }

                var dto = new CreateVolunteerHoursDto
                {
                    VolunteerRequestId = model.RequestId,
                    Date = model.Date,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    ActivitiesDescription = model.ActivitiesDescription,
                    Notes = model.Notes
                };

                return await _apiClient.PutAsync($"volunteerhours/{hoursId}", dto);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error al actualizar registro de horas: {ex.Message}");
            }
        }

        public async Task<Result> DeleteVolunteerHoursAsync(int hoursId)
        {
            try
            {
                // ✅ REQUERIMIENTO 2: Verificar que el registro existe antes de eliminar
                var existingHoursResult = await GetHoursByIdAsync(hoursId);
                if (existingHoursResult.IsFailure)
                {
                    return Result.Failure("Registro de horas no encontrado");
                }

                return await _apiClient.DeleteAsync($"volunteerhours/{hoursId}");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error al eliminar registro de horas: {ex.Message}");
            }
        }

        // ===== APROBACIÓN Y ADMINISTRACIÓN =====
        public async Task<Result> ApproveHoursAsync(int hoursId, int approverId, string approverName)
        {
            try
            {
                var dto = new ApproveHoursRequestDto
                {
                    ApproverId = approverId,
                    ApproverName = approverName
                };

                return await _apiClient.PostAsync($"volunteerhours/{hoursId}/approve", dto);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error al aprobar horas: {ex.Message}");
            }
        }

        public async Task<Result> RejectHoursAsync(int hoursId, int approverId, string approverName, string rejectionReason)
        {
            try
            {
                var dto = new RejectHoursRequestDto
                {
                    ApproverId = approverId,
                    ApproverName = approverName,
                    RejectionReason = rejectionReason
                };

                return await _apiClient.PostAsync($"volunteerhours/{hoursId}/reject", dto);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error al rechazar horas: {ex.Message}");
            }
        }

        public async Task<List<VolunteerHoursDto>> GetPendingHoursAsync()
        {
            try
            {
                var result = await _apiClient.GetAsync<List<VolunteerHoursDto>>("volunteerhours/pending");
                return result.IsSuccess ? (result.Value ?? new List<VolunteerHoursDto>()) : new List<VolunteerHoursDto>();
            }
            catch (Exception)
            {
                return new List<VolunteerHoursDto>();
            }
        }

        // ===== VALIDACIONES =====

        // ✅ REQUERIMIENTO 1: NUEVA VALIDACIÓN DE HORAS RESTANTES
        public async Task<Result> ValidateHoursWithRemainingHoursAsync(CreateVolunteerHoursViewModel model)
        {
            try
            {
                var errors = new List<string>();

                // Validaciones básicas primero
                var basicValidation = await ValidateHoursBasicAsync(model.Date, model.StartTime, model.EndTime, model.ActivitiesDescription, model.Notes);
                if (basicValidation.IsFailure)
                {
                    errors.AddRange(basicValidation.Errors);
                }

                // ✅ NUEVA VALIDACIÓN: Verificar horas restantes disponibles
                var totalHours = (decimal)(model.EndTime - model.StartTime).TotalHours;
                if (totalHours > model.RemainingHours)
                {
                    errors.Add($"No puedes registrar {totalHours:F1} horas. Solo quedan {model.RemainingHours:F1} horas disponibles de las {model.TotalHoursRequested} horas propuestas.");
                }

                if (model.RemainingHours <= 0)
                {
                    errors.Add("Ya has completado todas las horas comprometidas para esta solicitud.");
                }

                return errors.Any() ? Result.Failure(errors) : Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error en validación de horas: {ex.Message}");
            }
        }

        // ✅ REQUERIMIENTO 3: VERIFICAR REGISTROS EXISTENTES PARA LA FECHA
        public async Task<Result> CheckExistingHoursForDateAsync(int requestId, DateTime date)
        {
            try
            {
                var existingHours = await GetHoursByDateRangeAsync(requestId, date, date);
                if (existingHours.IsFailure)
                {
                    return Result.Success(); // Si no puede verificar, permitir continuar
                }

                var hoursForDate = existingHours.Value?.FirstOrDefault();
                if (hoursForDate != null)
                {
                    if (hoursForDate.State == VolunteerState.Approved)
                    {
                        return Result.Failure("Ya existe un registro de horas aprobado para esta fecha");
                    }

                    if (hoursForDate.State == VolunteerState.Pending)
                    {
                        return Result.Failure("Ya existe un registro de horas pendiente de aprobación para esta fecha");
                    }

                    // Si está rechazada, se permite re-registro (la API eliminará la anterior)
                    if (hoursForDate.State == VolunteerState.Rejected)
                    {
                        // Permitir continuar - se reemplazará la rechazada
                        return Result.Success();
                    }
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                // En caso de error, permitir continuar y que la API maneje la validación
                return Result.Success();
            }
        }

        // ✅ NUEVA VALIDACIÓN BÁSICA REUTILIZABLE
        private async Task<Result> ValidateHoursBasicAsync(DateTime date, TimeSpan startTime, TimeSpan endTime, string activitiesDescription, string? notes)
        {
            var errors = new List<string>();

            // Validar fecha
            if (date > DateTime.Today)
            {
                errors.Add("No se pueden registrar horas para fechas futuras");
            }

            if (date < DateTime.Today.AddDays(-30))
            {
                errors.Add("No se pueden registrar horas para fechas anteriores a 30 días");
            }

            // Validar horarios
            if (startTime >= endTime)
            {
                errors.Add("La hora de inicio debe ser menor a la hora de fin");
            }

            // Calcular total de horas
            var totalHours = (decimal)(endTime - startTime).TotalHours;

            // Validar máximo 8 horas
            if (totalHours > 8)
            {
                errors.Add("No se pueden registrar más de 8 horas por día");
            }

            // Validar mínimo 1 hora
            if (totalHours < 1)
            {
                errors.Add("Debe registrar al menos 1 hora de trabajo");
            }

            // Validar descripción de actividades
            if (string.IsNullOrWhiteSpace(activitiesDescription))
            {
                errors.Add("Debe describir las actividades realizadas");
            }

            if (activitiesDescription?.Length > 1000)
            {
                errors.Add("La descripción de actividades no puede exceder 1000 caracteres");
            }

            // Validar notas
            if (notes?.Length > 500)
            {
                errors.Add("Las notas no pueden exceder 500 caracteres");
            }

            return errors.Any() ? Result.Failure(errors) : Result.Success();
        }

        public async Task<Result> ValidateHoursAsync(AddHoursViewModel model)
        {
            try
            {
                var dto = new CreateVolunteerHoursDto
                {
                    VolunteerRequestId = model.RequestId,
                    Date = model.Date,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    ActivitiesDescription = model.ActivitiesDescription,
                    Notes = model.Notes
                };

                return await _apiClient.PostAsync("volunteerhours/validate", dto);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error al validar horas: {ex.Message}");
            }
        }

        public async Task<Result> ValidateHoursClientSideAsync(AddHoursViewModel model)
        {
            try
            {
                return await ValidateHoursBasicAsync(model.Date, model.StartTime, model.EndTime, model.ActivitiesDescription, model.Notes);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error en validación: {ex.Message}");
            }
        }

        public async Task<bool> HasHoursForDateAsync(int requestId, DateTime date)
        {
            try
            {
                var hoursResult = await GetHoursByDateRangeAsync(requestId, date, date);
                return hoursResult.IsSuccess && hoursResult.Value?.Any() == true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // ===== BÚSQUEDA Y FILTRADO BÁSICO =====
        public async Task<List<VolunteerHoursDto>> SearchHoursAsync(int requestId, string? searchTerm = null, VolunteerState? state = null)
        {
            try
            {
                var hoursResult = await GetHoursByRequestIdAsync(requestId);

                if (hoursResult.IsFailure || hoursResult.Value == null)
                {
                    return new List<VolunteerHoursDto>();
                }

                var filteredHours = hoursResult.Value.AsQueryable();

                // Filtro por término de búsqueda
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    searchTerm = searchTerm.ToLower();
                    filteredHours = filteredHours.Where(h =>
                        h.ActivitiesDescription.ToLower().Contains(searchTerm) ||
                        (h.Notes != null && h.Notes.ToLower().Contains(searchTerm)));
                }

                // Filtro por estado
                if (state.HasValue)
                {
                    filteredHours = filteredHours.Where(h => h.State == state.Value);
                }

                return filteredHours.OrderByDescending(h => h.Date).ToList();
            }
            catch (Exception)
            {
                return new List<VolunteerHoursDto>();
            }
        }

        // ===== MÉTODOS DE UTILIDAD =====
        public async Task<ManageHoursViewModel> BuildManageHoursViewModelAsync(int requestId, VolunteerRequestDto request)
        {
            try
            {
                var hoursResult = await GetHoursByRequestIdAsync(requestId);
                var hoursList = hoursResult.IsSuccess ? (hoursResult.Value ?? new List<VolunteerHoursDto>()) : new List<VolunteerHoursDto>();

                return new ManageHoursViewModel
                {
                    RequestId = requestId,
                    VolunteerName = request.VolunteerName ?? "N/A",
                    Institution = request.Institution,
                    HoursList = hoursList,
                    TotalHoursRequested = request.Hours,
                    CanAddMore = request.RemainingHours > 0
                };
            }
            catch (Exception)
            {
                return new ManageHoursViewModel
                {
                    RequestId = requestId,
                    VolunteerName = "Error",
                    Institution = "Error",
                    HoursList = new List<VolunteerHoursDto>(),
                    TotalHoursRequested = 0,
                    CanAddMore = false
                };
            }
        }

        // ✅ NUEVO MÉTODO: Construir ViewModel con datos de validación
        public async Task<CreateVolunteerHoursViewModel> BuildCreateHoursViewModelAsync(int requestId, VolunteerRequestDto request)
        {
            try
            {
                return new CreateVolunteerHoursViewModel
                {
                    VolunteerRequestId = requestId,
                    Date = DateTime.Today,
                    TotalHoursRequested = request.Hours,
                    TotalHoursApproved = request.HoursWorked
                };
            }
            catch (Exception)
            {
                return new CreateVolunteerHoursViewModel
                {
                    VolunteerRequestId = requestId,
                    Date = DateTime.Today,
                    TotalHoursRequested = 0,
                    TotalHoursApproved = 0
                };
            }
        }
    }
}
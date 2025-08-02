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

                // Obtener la solicitud para validar remaining hours
                var requestResult = await _apiClient.GetAsync<VolunteerRequestDto>($"VolunteerRequest/{model.RequestId}");
                if (!requestResult.IsSuccess || requestResult.Value == null)
                {
                    return Result.Failure("No se pudo obtener la solicitud de voluntariado para validar horas.");
                }

                var requestDto = requestResult.Value;

                if (requestDto.State != VolunteerState.Approved)
                {
                    return Result.Failure("Solo se pueden registrar horas para solicitudes aprobadas.");
                }

                if (requestDto.RemainingHours <= 0)
                {
                    return Result.Failure("No quedan horas disponibles en esta solicitud.");
                }

                var toAdd = (decimal)(model.EndTime - model.StartTime).TotalHours;
                if (toAdd > requestDto.RemainingHours)
                {
                    return Result.Failure($"No puedes registrar más de {requestDto.RemainingHours:F1} horas restantes.");
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

                return await _apiClient.PostAsync("VolunteerHours", dto);
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
                var result = await _apiClient.GetAsync<List<VolunteerHoursDto>>($"VolunteerHours/request/{requestId}");

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

                var url = $"VolunteerHours/request/{requestId}/date-range?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
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
                return await _apiClient.GetAsync<VolunteerHoursDto>($"VolunteerHours/{hoursId}");
            }
            catch (Exception ex)
            {
                return Result<VolunteerHoursDto>.Failure($"Error al obtener registro de horas: {ex.Message}");
            }
        }

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

                return await _apiClient.PutAsync($"VolunteerHours/{hoursId}", dto);
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
                return await _apiClient.DeleteAsync($"VolunteerHours/{hoursId}");
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

                return await _apiClient.PostAsync($"VolunteerHours/{hoursId}/approve", dto);
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

                return await _apiClient.PostAsync($"VolunteerHours/{hoursId}/reject", dto);
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
                var result = await _apiClient.GetAsync<List<VolunteerHoursDto>>("VolunteerHours/pending");
                return result.IsSuccess ? (result.Value ?? new List<VolunteerHoursDto>()) : new List<VolunteerHoursDto>();
            }
            catch (Exception)
            {
                return new List<VolunteerHoursDto>();
            }
        }

        public async Task<List<VolunteerHoursDto>> GetHoursByStateAsync(VolunteerState state)
        {
            try
            {
                var result = await _apiClient.GetAsync<List<VolunteerHoursDto>>($"VolunteerHours/state/{state}");
                return result.IsSuccess ? (result.Value ?? new List<VolunteerHoursDto>()) : new List<VolunteerHoursDto>();
            }
            catch (Exception)
            {
                return new List<VolunteerHoursDto>();
            }
        }

        // ===== VALIDACIONES =====
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

                return await _apiClient.PostAsync("VolunteerHours/validate", dto);
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
                var errors = new List<string>();

                // Validar fecha
                if (model.Date > DateTime.Today)
                {
                    errors.Add("No se pueden registrar horas para fechas futuras");
                }

                if (model.Date < DateTime.Today.AddDays(-30))
                {
                    errors.Add("No se pueden registrar horas para fechas anteriores a 30 días");
                }

                // Validar horarios
                if (model.StartTime >= model.EndTime)
                {
                    errors.Add("La hora de inicio debe ser menor a la hora de fin");
                }

                // Calcular total de horas
                var totalHours = (decimal)(model.EndTime - model.StartTime).TotalHours;

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

                // Validar horarios laborales
                if (model.StartTime < TimeSpan.FromHours(6) || model.EndTime > TimeSpan.FromHours(22))
                {
                    errors.Add("Los horarios deben estar entre 6:00 AM y 10:00 PM");
                }

                // Validar descripción de actividades
                if (string.IsNullOrWhiteSpace(model.ActivitiesDescription))
                {
                    errors.Add("Debe describir las actividades realizadas");
                }

                if (model.ActivitiesDescription?.Length > 1000)
                {
                    errors.Add("La descripción de actividades no puede exceder 1000 caracteres");
                }

                // Validar notas
                if (model.Notes?.Length > 500)
                {
                    errors.Add("Las notas no pueden exceder 500 caracteres");
                }

                return errors.Any() ? Result.Failure(errors) : Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error en validación: {ex.Message}");
            }
        }

        public async Task<bool> CanRegisterHoursAsync(int requestId, DateTime date)
        {
            try
            {
                var result = await _apiClient.GetAsync<Dictionary<string, object>>($"VolunteerHours/can-register/{requestId}?date={date:yyyy-MM-dd}");

                if (result.IsSuccess && result.Value != null && result.Value.ContainsKey("canRegister"))
                {
                    return Convert.ToBoolean(result.Value["canRegister"]);
                }

                return false;
            }
            catch (Exception)
            {
                return false;
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

        // ===== ESTADÍSTICAS Y REPORTES =====

        // ===== BÚSQUEDA Y FILTRADO =====
        public async Task<List<VolunteerHoursDto>> SearchHoursAsync(int requestId, string searchTerm = null, VolunteerState? state = null, DateTime? startDate = null, DateTime? endDate = null)
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

                // Filtro por fechas
                if (startDate.HasValue)
                {
                    filteredHours = filteredHours.Where(h => h.Date >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    filteredHours = filteredHours.Where(h => h.Date <= endDate.Value);
                }

                return filteredHours.OrderByDescending(h => h.Date).ToList();
            }
            catch (Exception)
            {
                return new List<VolunteerHoursDto>();
            }
        }

        // ===== EXPORTACIÓN =====
        public async Task<Result<byte[]>> ExportHoursAsync(int requestId, string format = "excel", DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var url = $"VolunteerHours/request/{requestId}/export?format={format}";

                if (startDate.HasValue)
                    url += $"&startDate={startDate.Value:yyyy-MM-dd}";

                if (endDate.HasValue)
                    url += $"&endDate={endDate.Value:yyyy-MM-dd}";

                return await _apiClient.GetAsync<byte[]>(url);
            }
            catch (Exception ex)
            {
                return Result<byte[]>.Failure($"Error al exportar: {ex.Message}");
            }
        }

        // ===== MÉTODOS AUXILIARES =====
        private static int GetWeekOfYear(DateTime date)
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            return culture.Calendar.GetWeekOfYear(date, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
        }

        private static DateTime GetWeekStartDate(int year, int weekNumber)
        {
            var jan1 = new DateTime(year, 1, 1);
            var daysOffset = (int)System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek - (int)jan1.DayOfWeek;
            var firstWeek = jan1.AddDays(daysOffset);
            return firstWeek.AddDays((weekNumber - 1) * 7);
        }

        public async Task<TimeSpan> GetAverageWorkTimeAsync(int requestId)
        {
            try
            {
                var hoursResult = await GetHoursByRequestIdAsync(requestId);

                if (hoursResult.IsFailure || hoursResult.Value == null || !hoursResult.Value.Any())
                {
                    return TimeSpan.Zero;
                }

                var approvedHours = hoursResult.Value.Where(h => h.State == VolunteerState.Approved);

                if (!approvedHours.Any())
                    return TimeSpan.Zero;

                var averageStart = TimeSpan.FromTicks((long)approvedHours.Average(h => h.StartTime.Ticks));
                var averageEnd = TimeSpan.FromTicks((long)approvedHours.Average(h => h.EndTime.Ticks));

                return averageEnd - averageStart;
            }
            catch (Exception)
            {
                return TimeSpan.Zero;
            }
        }
    }
}

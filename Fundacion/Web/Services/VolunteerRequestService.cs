using Shared.Dtos.Volunteer;
using Shared.Enums;
using Shared.Models;
using Web.Http;
using Web.Models.Volunteer;

namespace Web.Services
{
    public class VolunteerRequestService
    {
        private readonly ApiClient _apiClient;

        public VolunteerRequestService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        // ===== GESTIÓN BÁSICA DE SOLICITUDES =====
        public async Task<Result> CreateAsync(CreateRequestViewModel requestViewModel, int volunteerId)
        {
            try
            {
                var requestDto = new VolunteerRequestDto
                {
                    VolunteerId = volunteerId,
                    Institution = requestViewModel.Institution,
                    Profession = requestViewModel.Profession,
                    Description = requestViewModel.Description,
                    Hours = requestViewModel.Hours
                };

                return await _apiClient.PostAsync("volunteerrequest", requestDto);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error al crear la solicitud: {ex.Message}");
            }
        }

        public async Task<List<VolunteerRequestDto>> GetAllByVolunteerIDAsync(int volunteerId)
        {
            try
            {
                var result = await _apiClient.GetAsync<List<VolunteerRequestDto>>($"volunteerrequest/volunteer/{volunteerId}");

                if (result.IsSuccess)
                {
                    return result.Value ?? new List<VolunteerRequestDto>();
                }

                // Log del error si es necesario
                return new List<VolunteerRequestDto>();
            }
            catch (Exception)
            {
                // En caso de excepción, retornar lista vacía
                return new List<VolunteerRequestDto>();
            }
        }

        public async Task<Result<VolunteerRequestDto>> GetRequestByIdAsync(int requestId)
        {
            try
            {
                return await _apiClient.GetAsync<VolunteerRequestDto>($"VolunteerRequest/{requestId}");
            }
            catch (Exception ex)
            {
                return Result<VolunteerRequestDto>.Failure($"Error al obtener la solicitud: {ex.Message}");
            }
        }

        // ===== MÉTODOS DE ADMINISTRACIÓN =====
        public async Task<Result> ApproveRequestAsync(int requestId, int approverId)
        {
            try
            {
                // ✅ CORRECCIÓN: Crear el DTO correcto
                var approveDto = new ApproveRequestDto
                {
                    ApproverId = approverId,
                    ApproverName = "Administrador" // O puedes pasar el nombre real como parámetro
                };

                return await _apiClient.PostAsync($"VolunteerRequest/{requestId}/approve", approveDto);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error al aprobar la solicitud: {ex.Message}");
            }
        }

        public async Task<Result> RejectRequestAsync(int requestId, int approverId, string reason)
        {
            try
            {
                // ✅ CORRECCIÓN: Agregado ApproverName
                var rejectDto = new RejectRequestDto
                {
                    ApproverId = approverId,
                    ApproverName = "Administrador", // Agregado campo faltante
                    Reason = reason
                };

                return await _apiClient.PostAsync($"VolunteerRequest/{requestId}/reject", rejectDto);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error al rechazar la solicitud: {ex.Message}");
            }
        }

        public async Task<List<VolunteerRequestDto>> GetAllRequestsAsync()
        {
            try
            {
                var result = await _apiClient.GetAsync<List<VolunteerRequestDto>>("VolunteerRequest");
                return result.IsSuccess ? (result.Value ?? new List<VolunteerRequestDto>()) : new List<VolunteerRequestDto>();
            }
            catch (Exception)
            {
                return new List<VolunteerRequestDto>();
            }
        }

        public async Task<List<VolunteerRequestDto>> GetRequestsByStateAsync(VolunteerState state)
        {
            try
            {
                var result = await _apiClient.GetAsync<List<VolunteerRequestDto>>($"VolunteerRequest/state/{state}");
                return result.IsSuccess ? (result.Value ?? new List<VolunteerRequestDto>()) : new List<VolunteerRequestDto>();
            }
            catch (Exception)
            {
                return new List<VolunteerRequestDto>();
            }
        }

        // ===== MÉTODOS DE VALIDACIÓN =====
        public async Task<Result> ValidateRequestAsync(CreateRequestViewModel model)
        {
            try
            {
                // Validaciones del lado del cliente antes de enviar al API
                var errors = new List<string>();

                if (string.IsNullOrWhiteSpace(model.Institution))
                    errors.Add("La institución es obligatoria");

                if (string.IsNullOrWhiteSpace(model.Profession))
                    errors.Add("La profesión es obligatoria");

                if (string.IsNullOrWhiteSpace(model.Description))
                    errors.Add("La descripción es obligatoria");

                if (model.Hours <= 0)
                    errors.Add("Las horas deben ser mayor a 0");

                if (model.Hours > 1000)
                    errors.Add("Las horas no pueden exceder 1000");

                if (errors.Any())
                    return Result.Failure(errors);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error en la validación: {ex.Message}");
            }
        }

        public async Task<bool> CanCreateNewRequestAsync(int volunteerId)
        {
            try
            {
                var existingRequests = await GetAllByVolunteerIDAsync(volunteerId);

                // Verificar si ya tiene una solicitud activa (Pendiente o Aprobada)
                var hasActiveRequest = existingRequests.Any(r =>
                    r.State == VolunteerState.Pending ||
                    r.State == VolunteerState.Approved);

                return !hasActiveRequest;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // ===== MÉTODOS DE UTILIDAD =====
        public async Task<Result<VolunteerRequestDto>> GetActiveRequestAsync(int volunteerId)
        {
            try
            {
                var requests = await GetAllByVolunteerIDAsync(volunteerId);
                var activeRequest = requests.FirstOrDefault(r =>
                    r.State == VolunteerState.Pending ||
                    r.State == VolunteerState.Approved);

                if (activeRequest == null)
                {
                    return Result<VolunteerRequestDto>.Failure("No se encontró una solicitud activa");
                }

                return Result<VolunteerRequestDto>.Success(activeRequest);
            }
            catch (Exception ex)
            {
                return Result<VolunteerRequestDto>.Failure($"Error al obtener solicitud activa: {ex.Message}");
            }
        }

        public async Task<int> GetTotalRequestsCountAsync(int volunteerId)
        {
            try
            {
                var requests = await GetAllByVolunteerIDAsync(volunteerId);
                return requests.Count;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> GetApprovedRequestsCountAsync(int volunteerId)
        {
            try
            {
                var requests = await GetAllByVolunteerIDAsync(volunteerId);
                return requests.Count(r => r.State == VolunteerState.Approved);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> GetTotalHoursCommittedAsync(int volunteerId)
        {
            try
            {
                var requests = await GetAllByVolunteerIDAsync(volunteerId);
                return requests
                    .Where(r => r.State == VolunteerState.Approved)
                    .Sum(r => r.Hours);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        // ===== MÉTODOS PARA ADMINISTRADORES =====
        public async Task<List<VolunteerRequestDto>> GetPendingRequestsAsync()
        {
            return await GetRequestsByStateAsync(VolunteerState.Pending);
        }

        public async Task<List<VolunteerRequestDto>> GetApprovedRequestsAsync()
        {
            return await GetRequestsByStateAsync(VolunteerState.Approved);
        }

        public async Task<List<VolunteerRequestDto>> GetRejectedRequestsAsync()
        {
            return await GetRequestsByStateAsync(VolunteerState.Rejected);
        }

        // ===== MÉTODOS DE BÚSQUEDA Y FILTRADO =====
        public async Task<List<VolunteerRequestDto>> SearchRequestsAsync(string searchTerm, VolunteerState? state = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var allRequests = await GetAllRequestsAsync();
                var filteredRequests = allRequests.AsQueryable();

                // Filtro por término de búsqueda
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    searchTerm = searchTerm.ToLower();
                    filteredRequests = filteredRequests.Where(r =>
                        r.VolunteerName != null && r.VolunteerName.ToLower().Contains(searchTerm) ||
                        r.Institution.ToLower().Contains(searchTerm) ||
                        r.Profession.ToLower().Contains(searchTerm) ||
                        r.Description.ToLower().Contains(searchTerm));
                }

                // Filtro por estado
                if (state.HasValue)
                {
                    filteredRequests = filteredRequests.Where(r => r.State == state.Value);
                }

                // Filtro por fechas
                if (startDate.HasValue)
                {
                    filteredRequests = filteredRequests.Where(r => r.CreatedAt >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    filteredRequests = filteredRequests.Where(r => r.CreatedAt <= endDate.Value);
                }

                return filteredRequests.ToList();
            }
            catch (Exception)
            {
                return new List<VolunteerRequestDto>();
            }
        }

        // ===== MÉTODOS DE EXPORTACIÓN =====
        public async Task<Result<byte[]>> ExportRequestsAsync(string format = "excel", VolunteerState? state = null)
        {
            try
            {
                var url = $"VolunteerRequest/export?format={format}";
                if (state.HasValue)
                {
                    url += $"&state={state.Value}";
                }

                return await _apiClient.GetAsync<byte[]>(url);
            }
            catch (Exception ex)
            {
                return Result<byte[]>.Failure($"Error al exportar: {ex.Message}");
            }
        }

        // ===== MÉTODOS MEJORADOS CON NOMBRES DE APROBADORES =====

        /// <summary>
        /// Método mejorado que acepta el nombre del aprobador
        /// </summary>
        public async Task<Result> ApproveRequestAsync(int requestId, int approverId, string approverName)
        {
            try
            {
                var approveDto = new ApproveRequestDto
                {
                    ApproverId = approverId,
                    ApproverName = approverName
                };

                return await _apiClient.PostAsync($"VolunteerRequest/{requestId}/approve", approveDto);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error al aprobar la solicitud: {ex.Message}");
            }
        }

        /// <summary>
        /// Método mejorado que acepta el nombre del aprobador
        /// </summary>
        public async Task<Result> RejectRequestAsync(int requestId, int approverId, string approverName, string reason)
        {
            try
            {
                var rejectDto = new RejectRequestDto
                {
                    ApproverId = approverId,
                    ApproverName = approverName,
                    Reason = reason
                };

                return await _apiClient.PostAsync($"VolunteerRequest/{requestId}/reject", rejectDto);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error al rechazar la solicitud: {ex.Message}");
            }
        }
    }
}
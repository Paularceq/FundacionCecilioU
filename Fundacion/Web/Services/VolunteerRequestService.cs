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
                // CORREGIDO: Usar CreateVolunteerRequestDto en lugar de VolunteerRequestDto
                var requestDto = new CreateVolunteerRequestDto
                {
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
                return result.IsSuccess ? (result.Value ?? new List<VolunteerRequestDto>()) : new List<VolunteerRequestDto>();
            }
            catch (Exception)
            {
                return new List<VolunteerRequestDto>();
            }
        }

        public async Task<Result<VolunteerRequestDto>> GetRequestByIdAsync(int requestId)
        {
            try
            {
                return await _apiClient.GetAsync<VolunteerRequestDto>($"volunteerrequest/{requestId}");
            }
            catch (Exception ex)
            {
                return Result<VolunteerRequestDto>.Failure($"Error al obtener la solicitud: {ex.Message}");
            }
        }

        // ===== MÉTODOS DE ADMINISTRACIÓN =====
        public async Task<Result> ApproveRequestAsync(int requestId, int approverId, string approverName)
        {
            try
            {
                var approveDto = new ApproveRequestDto
                {
                    ApproverId = approverId,
                    ApproverName = approverName
                };

                return await _apiClient.PostAsync($"volunteerrequest/{requestId}/approve", approveDto);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error al aprobar la solicitud: {ex.Message}");
            }
        }

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

                return await _apiClient.PostAsync($"volunteerrequest/{requestId}/reject", rejectDto);
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
                var result = await _apiClient.GetAsync<List<VolunteerRequestDto>>("volunteerrequest");
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
                var result = await _apiClient.GetAsync<List<VolunteerRequestDto>>($"volunteerrequest/state/{state}");
                return result.IsSuccess ? (result.Value ?? new List<VolunteerRequestDto>()) : new List<VolunteerRequestDto>();
            }
            catch (Exception)
            {
                return new List<VolunteerRequestDto>();
            }
        }

        // ===== MÉTODOS DE VALIDACIÓN =====
        public async Task<bool> CanCreateNewRequestAsync(int volunteerId)
        {
            try
            {
                var existingRequests = await GetAllByVolunteerIDAsync(volunteerId);

                // REQUERIMIENTO: No puede haber más de una solicitud activa por voluntario
                // PERO puede crear nueva si ya cumplió las horas de la anterior
                var activeRequest = existingRequests.FirstOrDefault(r =>
                    r.State == VolunteerState.Pending ||
                    (r.State == VolunteerState.Approved && r.RemainingHours > 0));

                return activeRequest == null;
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

        // ===== MÉTODOS DE BÚSQUEDA BÁSICA =====
        public async Task<List<VolunteerRequestDto>> SearchRequestsAsync(string? searchTerm = null, VolunteerState? state = null)
        {
            try
            {
                var allRequests = state.HasValue
                    ? await GetRequestsByStateAsync(state.Value)
                    : await GetAllRequestsAsync();

                if (string.IsNullOrWhiteSpace(searchTerm))
                    return allRequests;

                searchTerm = searchTerm.ToLower();
                return allRequests.Where(r =>
                    (r.VolunteerName?.ToLower().Contains(searchTerm) ?? false) ||
                    r.Institution.ToLower().Contains(searchTerm) ||
                    r.Profession.ToLower().Contains(searchTerm) ||
                    r.Description.ToLower().Contains(searchTerm)
                ).ToList();
            }
            catch (Exception)
            {
                return new List<VolunteerRequestDto>();
            }

        }

    }
}
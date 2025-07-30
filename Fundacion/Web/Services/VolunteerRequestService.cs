using Shared.Dtos.Volunteer;
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
        public async Task<Result> CreateAsync(CreateRequestViewModel requestViewModel, int volunterId)
        {
            var requestDto = new VolunteerRequestDto
            {
                VolunteerId = volunterId,
                Institution = requestViewModel.Institution,
                Profession = requestViewModel.Profession,
                Description = requestViewModel.Description,
                Hours = requestViewModel.Hours
            };
            return await _apiClient.PostAsync("volunteerrequest", requestDto);
        }
        public async Task<List<VolunteerRequestDto>> GetAllByVolunteerIDAsync(int volunteerId)
        {
            var result = await _apiClient.GetAsync<List<VolunteerRequestDto>>($"volunteerrequest/volunteer/{volunteerId}");
            return result.Value;
        }
        // ===== NUEVOS MÉTODOS =====


        public async Task<Result<VolunteerRequestDto>> GetRequestByIdAsync(int requestId)
        {
            return await _apiClient.GetAsync<VolunteerRequestDto>($"VolunteerRequest/{requestId}");
        }

    }


}

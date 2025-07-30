using Shared.Dtos.Volunteer;
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

        public async Task<Result> CreateVolunteerHoursAsync(AddHoursViewModel model)
        {
            var dto = new CreateVolunteerHoursDto
            {
                VolunteerRequestId = model.RequestId,
                Date = model.Date,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                ActivitiesDescription = model.ActivitiesDescription,
                Notes = model.Notes,
               
            };

            return await _apiClient.PostAsync("VolunteerHours", dto);
        }

        public async Task<Result<List<VolunteerHoursDto>>> GetHoursByRequestIdAsync(int requestId)
        {
            return await _apiClient.GetAsync<List<VolunteerHoursDto>>($"VolunteerHours/request/{requestId}");
        }

        public async Task<Result<List<VolunteerHoursDto>>> GetHoursByDateRangeAsync(int requestId, DateTime startDate, DateTime endDate)
        {
            var url = $"VolunteerHours/request/{requestId}/date-range?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
            return await _apiClient.GetAsync<List<VolunteerHoursDto>>(url);
        }

        public async Task<Result<VolunteerHoursDto>> GetHoursByIdAsync(int hoursId)
        {
            return await _apiClient.GetAsync<VolunteerHoursDto>($"VolunteerHours/{hoursId}");
        }

        public async Task<Result> UpdateVolunteerHoursAsync(int hoursId, AddHoursViewModel model)
        {
            var dto = new CreateVolunteerHoursDto
            {
                VolunteerRequestId = model.RequestId,
                Date = model.Date,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                ActivitiesDescription = model.ActivitiesDescription,
                Notes = model.Notes,
               
            };

            return await _apiClient.PutAsync($"VolunteerHours/{hoursId}", dto);
        }

        public async Task<Result> DeleteVolunteerHoursAsync(int hoursId)
        {
            return await _apiClient.DeleteAsync($"VolunteerHours/{hoursId}");
        }

        public async Task<Result> ValidateHoursAsync(AddHoursViewModel model)
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

        public async Task<bool> CanRegisterHoursAsync(int requestId, DateTime date)
        {
            var result = await _apiClient.GetAsync<dynamic>($"VolunteerRequest/{requestId}/can-register-hours?date={date:yyyy-MM-dd}");
            return result.IsSuccess && result.Value?.canRegister == true;
        }
    }
}
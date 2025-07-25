using Api.Database.Entities;

namespace Api.Abstractions.Repositories
{
    public interface IVolunteerRequestRepository
    {
        Task CreateRequest(VolunteerRequest voluteerRequest);
        Task<VolunteerRequest> GetActiveRequest(int VolunteerId);
        Task<List<VolunteerRequest>> GetRequestsByVolunteerID(int volunteerID);
    }
}
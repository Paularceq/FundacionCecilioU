using Shared.Dtos.Volunteer;
namespace Web.Models.Volunteer
{
    public class VolunteerRequestListViewModel
    {
        public List<VolunteerRequestDto> Requests { get; set; } = new();
        public bool CanCreateNew { get; set; } = true;

        // Estadísticas básicas
        public int TotalRequests => Requests.Count;
        public int PendingRequests => Requests.Count(r => r.State == Shared.Enums.VolunteerState.Pending);
        public int ApprovedRequests => Requests.Count(r => r.State == Shared.Enums.VolunteerState.Approved);
        public int CompletedRequests => Requests.Count(r => r.RemainingHours <= 0);
        public bool HasActiveRequest => Requests.Any(r =>
            r.State == Shared.Enums.VolunteerState.Pending ||
            r.State == Shared.Enums.VolunteerState.Approved);
    }
}
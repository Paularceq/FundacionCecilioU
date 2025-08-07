using Shared.Dtos.Volunteer;
using Shared.Enums;
namespace Web.Models.Volunteer
{
    public class AdminVolunteerViewModel
    {
        public List<VolunteerRequestDto> PendingRequests { get; set; } = new();
        public List<VolunteerRequestDto> ApprovedRequests { get; set; } = new();
        public List<VolunteerRequestDto> RejectedRequests { get; set; } = new();
        public List<VolunteerHoursDto> PendingHours { get; set; } = new();

        // Estadísticas básicas para dashboard
        public int TotalActiveVolunteers => ApprovedRequests.Count;
        public int PendingRequestsCount => PendingRequests.Count;
        public int PendingHoursCount => PendingHours.Count;

        // Filtros básicos
        public VolunteerState? FilterState { get; set; }
        public DateTime? FilterStartDate { get; set; }
        public DateTime? FilterEndDate { get; set; }
        public string? SearchTerm { get; set; }
    }
}
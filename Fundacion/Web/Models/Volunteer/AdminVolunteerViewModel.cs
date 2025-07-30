using Shared.Dtos.Volunteer;
using Shared.Enums;

namespace Web.Models.Volunteer
{
    public class AdminVolunteerViewModel
    {
        public List<VolunteerRequestDto> PendingRequests { get; set; } = new();
        public List<VolunteerRequestDto> ApprovedRequests { get; set; } = new();
        public List<VolunteerHoursDto> PendingHours { get; set; } = new();

        // Estadísticas para dashboard de admin
        public int TotalVolunteers => ApprovedRequests.Count;
        public int PendingApprovals => PendingRequests.Count;
        public int PendingHoursApprovals => PendingHours.Count;
        public decimal TotalHoursThisMonth => PendingHours
            .Where(h => h.Date.Month == DateTime.Now.Month && h.Date.Year == DateTime.Now.Year)
            .Sum(h => h.TotalHours);

        // Filtros
        public VolunteerState? FilterState { get; set; }
        public DateTime? FilterStartDate { get; set; }
        public DateTime? FilterEndDate { get; set; }
        public string? SearchTerm { get; set; }
    }
}

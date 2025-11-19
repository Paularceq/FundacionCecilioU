using Shared.Enums;

namespace Api.Database.Entities
{
    public class VolunteerHours
    {
        public int Id { get; set; }
        public int? ApproverId { get; set; }
        public User? Approver { get; set; }
        public int VolunteerRequestId { get; set; }
        public VolunteerRequest VolunteerRequest { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public decimal TotalHours { get; set; } // Calculado automáticamente

        // Actividades realizadas
        public string ActivitiesDescription { get; set; } = string.Empty;
        public string? Notes { get; set; }

        // Control de estado
        public VolunteerState State { get; set; } = VolunteerState.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ApprovedAt { get; set; }
        public string? RejectionReason { get; set; }

    }
}




using Shared.Enums;

namespace Shared.Dtos.Volunteer
{
    public class VolunteerRequestDto
    {
        public int Id { get; set; }
        public int VolunteerId { get; set; }
        public string? VolunteerName { get; set; }
        public int? ApproverId { get; set; }
        public string? ApproverName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public VolunteerState State { get; set; }
        public string Institution { get; set; } = string.Empty;
        public string Profession { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Hours { get; set; }
        public string? RejectionReason { get; set; } // Nueva propiedad
    }
}
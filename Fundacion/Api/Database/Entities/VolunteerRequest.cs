using Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Api.Database.Entities
{
    public class VolunteerRequest
    {
        public int Id { get; set; }
        public int VolunteerId { get; set; }
        public User Volunteer { get; set; }
        public int? ApproverId { get; set; }
        public User Approver { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ApprovedAt { get; set; } // Nueva propiedad para fecha de aprobación/rechazo
        public VolunteerState State { get; set; }
        public string Institution { get; set; }
        public string Profession { get; set; }
        public string Description { get; set; }
        public int Hours { get; set; }

        // Nueva propiedad para la razón del rechazo
        [MaxLength(500)]
        public string? RejectionReason { get; set; }
    }
}
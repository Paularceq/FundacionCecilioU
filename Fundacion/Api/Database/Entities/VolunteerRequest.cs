using Shared.Enums;

namespace Api.Database.Entities
{
    public class VolunteerRequest
    {
        public int Id { get; set; }
        public int VolunteerId { get; set; }
        public User Volunteer { get; set; }
        public int? ApproverId { get; set; }
        public User Approver { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public VolunteerState State { get; set; }
        public string Institution { get; set; }
        public string Profession { get; set; }
        public string Description { get; set; }
        public int Hours { get; set; }



    }
}

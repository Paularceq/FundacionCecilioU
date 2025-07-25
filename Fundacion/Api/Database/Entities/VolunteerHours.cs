namespace Api.Database.Entities
{
    public class VolunteerHours
    {
        public int Id { get; set; }
        public int ApproverId { get; set; }
        public User Approver { get; set; }
        public int VolunteerRequestId { get; set; }
        public VolunteerRequest VolunteerRequest { get; set; }
        public DateOnly Date { get; set; }
        public int Hours { get; set; }
        public string Description { get; set; }

    }
}

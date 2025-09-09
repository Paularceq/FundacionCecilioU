using Shared.Enums;

namespace Api.Database.Entities
{
    public class Scholarship
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public SolicitudBeca Request { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public ScholarshipFrequency Frequency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? LastPayment { get; set; }
        public bool IsActive { get; set; }
    }
}

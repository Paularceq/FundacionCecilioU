using Shared.Enums;

namespace Shared.Dtos.Becas
{
    public class TomarDesicionDto
    {
        public string Estado { get; set; }
        public decimal? Amount { get; set; }
        public Currency? Currency { get; set; }
        public ScholarshipFrequency? Frequency { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}

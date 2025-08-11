using Shared.Enums;

namespace Shared.Dtos.Financial
{
    public class BudgetDto
    {
        public int Id { get; set; }
        public decimal OriginalAmount { get; set; }
        public Currency Currency { get; set; }
        public decimal OriginalAmountInCRC { get; set; }
        public decimal RemainingAmountInCRC { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}

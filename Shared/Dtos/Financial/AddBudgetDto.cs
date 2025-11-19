using Shared.Enums;

namespace Shared.Dtos.Financial
{
    public class AddBudgetDto
    {
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
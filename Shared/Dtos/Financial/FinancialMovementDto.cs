using Shared.Enums;

namespace Shared.Dtos.Financial
{
    public class FinancialMovementDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public DateTime Date { get; set; }
        public MovementType Type { get; set; }
        public int CreatedById { get; set; }
        public string CreatedByName { get; set; }
    }
}

using Shared.Dtos.Financial;

namespace Web.Models.Financial
{
    public class FinancialMovementsFilterViewModel
    {
        public DateTime From { get; set; } = DateTime.Today.AddMonths(-1);
        public DateTime To { get; set; } = DateTime.Today;
        public List<FinancialMovementDto> Movements { get; set; } = [];
    }
}

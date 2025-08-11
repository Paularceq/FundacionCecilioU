namespace Shared.Dtos.Financial
{
    public class FinancialDashboardDto
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal OriginalBudgetAmmount { get; set; }
        public decimal CurrentBudgetBalance { get; set; }
        public decimal BudgetExecutionPercentage { get; set; }
        public List<MonthlyComparisonDto> MonthlyComparison { get; set; }
    }
}

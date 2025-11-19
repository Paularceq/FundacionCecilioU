using Api.Database.Entities;
using Shared.Dtos.Financial;

namespace Api.Abstractions.Repositories
{
    public interface IFinancialRepository
    {
        Task AddBudgetAsync(Budget budget);
        Task AddExpenseAsync(Expense expense);
        Task AddFinancialMovementAsync(FinancialMovement movement);
        Task AddLeaseAsync(Lease lease);
        Task<List<Budget>> GetAllBudgetsAsync();
        Task<BudgetDto> GetBudgetForDateAsync(DateTime date);
        Task<List<FinancialMovement>> GetFinancialMovementsAsync(DateTime from, DateTime to);
    }
}
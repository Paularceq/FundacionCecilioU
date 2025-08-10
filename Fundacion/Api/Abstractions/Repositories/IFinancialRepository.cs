using Api.Database.Entities;
using Shared.Dtos.Financial;

namespace Api.Abstractions.Repositories
{
    public interface IFinancialRepository
    {
        Task AddExpenseAsync(Expense expense);
        Task AddFinancialMovementAsync(FinancialMovement movement);
        Task AddLeaseAsync(Lease lease);
        Task<BudgetDto> GetBudgetForDateAsync(DateTime date);
    }
}
using Api.Database.Entities;
using Shared.Dtos.Financial;
using Shared.Models;

namespace Api.Abstractions.Application
{
    public interface IFinancialService
    {
        Task<Result> AddBudgetAsync(AddBudgetDto dto);
        Task<Result> AddExpenseAsync(AdministrativeExpenseDto expenseDto);
        Task<Result> AddLeaseIncomeAsync(LeaseIncomeDto leaseIncomeDto);
        Task<List<BudgetDto>> GetAllBudgetsAsync();
        Task<Result<FinancialDashboardDto>> GetDashboardAsync();
        Task<List<FinancialMovementDto>> GetMovementsByDateRangeAsync(DateTime from, DateTime to);
    }
}
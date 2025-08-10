using Shared.Dtos.Financial;
using Shared.Models;

namespace Api.Abstractions.Application
{
    public interface IFinancialService
    {
        Task<Result> AddExpenseAsync(AdministrativeExpenseDto expenseDto);
        Task<Result> AddLeaseIncomeAsync(LeaseIncomeDto leaseIncomeDto);
    }
}
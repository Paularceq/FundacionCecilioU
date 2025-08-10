using Api.Abstractions.Infrastructure;
using Api.Abstractions.Repositories;
using Api.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Dtos.Financial;
using Shared.Enums;

namespace Api.Database.Repositories
{
    public class FinancialRepository : IFinancialRepository
    {
        private readonly DatabaseContext _context;
        private readonly IExchangeRateService _exchangeRateService;

        public FinancialRepository(DatabaseContext context, IExchangeRateService exchangeRateService)
        {
            _context = context;
            _exchangeRateService = exchangeRateService;
        }

        public async Task AddFinancialMovementAsync(FinancialMovement movement)
        {
            _context.FinancialMovements.Add(movement);
            await _context.SaveChangesAsync();
        }

        public async Task AddLeaseAsync(Lease lease)
        {
            _context.Leases.Add(lease);
            await _context.SaveChangesAsync();
        }

        public async Task AddExpenseAsync(Expense expense)
        {
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
        }

        public async Task<BudgetDto> GetBudgetForDateAsync(DateTime date)
        {
            var budget = await _context.Budgets
                .Where(b => b.StartDate <= date && b.EndDate >= date)
                .FirstOrDefaultAsync();

            if (budget == null)
                return null;

            var movements = await _context.FinancialMovements
                .Where(m => m.Date >= budget.StartDate && m.Date <= budget.EndDate)
                .ToListAsync();

            var exchangeRate = await _exchangeRateService.GetExchangeRateForCRCAsync(budget.Currency);
            decimal remaining = budget.Amount * exchangeRate;

            foreach (var m in movements)
            {
                exchangeRate = await _exchangeRateService.GetExchangeRateForCRCAsync(m.Currency);

                if (m.Type == MovementType.Inbound)
                    remaining += m.Amount * exchangeRate;
                else if (m.Type == MovementType.Outbound)
                    remaining -= m.Amount * exchangeRate;
            }

            return new BudgetDto
            {
                Id = budget.Id,
                OriginalAmountInCRC = budget.Amount,
                RemainingAmountInCRC = remaining,
                StartDate = budget.StartDate,
                EndDate = budget.EndDate
            };
        }

    }
}

using Api.Abstractions.Application;
using Api.Abstractions.Infrastructure;
using Api.Abstractions.Repositories;
using Api.Database.Entities;
using Shared.Dtos.Financial;
using Shared.Enums;
using Shared.Models;

namespace Api.Services.Application
{
    public class FinancialService : IFinancialService
    {
        private readonly IFinancialRepository _financialRepository;
        private readonly IExchangeRateService _exchangeRateService;

        public FinancialService(
            IFinancialRepository financialRepository,
            IExchangeRateService exchangeRateService)
        {
            _financialRepository = financialRepository;
            _exchangeRateService = exchangeRateService;
        }

        public async Task<Result> AddLeaseIncomeAsync(LeaseIncomeDto leaseIncomeDto)
        {
            var financialMovement = new FinancialMovement
            {
                Description = $"Ingreso de alquiler para {leaseIncomeDto.TenantName}",
                Amount = leaseIncomeDto.Amount,
                Date = DateTime.UtcNow,
                Type = MovementType.Inbound,
                CreatedById = leaseIncomeDto.CreatedById
            };

            await _financialRepository.AddFinancialMovementAsync(financialMovement);

            var lease = new Lease
            {
                TenantName = leaseIncomeDto.TenantName,
                TenantIdentification = leaseIncomeDto.TenantIdentification,
                Usage = leaseIncomeDto.Usage,
                StartDate = leaseIncomeDto.StartDate,
                EndDate = leaseIncomeDto.EndDate,
                ReceiptBytes = leaseIncomeDto.ReceiptBytes,
                ReceiptContentType = leaseIncomeDto.ReceiptContentType,
                ReceiptFileName = leaseIncomeDto.ReceiptFileName,
                FinancialMovementId = financialMovement.Id
            };

            await _financialRepository.AddLeaseAsync(lease);

            return Result.Success();
        }

        public async Task<Result> AddExpenseAsync(AdministrativeExpenseDto expenseDto)
        {
            var budget = await _financialRepository.GetBudgetForDateAsync(expenseDto.Date);
            if (budget == null)
            {
                return Result.Failure("No se encontró un presupuesto para la fecha especificada.");
            }

            var amountInCRC = await _exchangeRateService.GetAmountInCRCAsync(expenseDto.Amount, expenseDto.Currency);
            if (amountInCRC > budget.RemainingAmountInCRC)
            {
                return Result.Failure("El monto del gasto excede el presupuesto disponible.");
            }

            var financialMovement = new FinancialMovement
            {
                Description = expenseDto.Description,
                Amount = expenseDto.Amount,
                Currency = expenseDto.Currency,
                Date = expenseDto.Date,
                Type = MovementType.Outbound,
                CreatedById = expenseDto.CreatedById
            };
            await _financialRepository.AddFinancialMovementAsync(financialMovement);

            var expense = new Expense
            {
                FinancialMovementId = financialMovement.Id,
                Description = expenseDto.Description,
                Date = DateTime.UtcNow,
                ReceiptBytes = expenseDto.ReceiptBytes,
                ReceiptContentType = expenseDto.ReceiptContentType,
                ReceiptFileName = expenseDto.ReceiptFileName
            };
            await _financialRepository.AddExpenseAsync(expense);

            return Result.Success();
        }

        public async Task<Result<FinancialDashboardDto>> GetDashboardAsync()
        {
            var budget = await _financialRepository.GetBudgetForDateAsync(DateTime.UtcNow);
            if (budget == null)
                return Result<FinancialDashboardDto>.Failure("No se encontró un presupuesto para el periodo actual.");

            var movements = await _financialRepository.GetFinancialMovementsAsync(budget.StartDate, budget.EndDate);

            var totalIncomeInCRC = 0m;
            var totalExpenseInCRC = 0m;

            foreach (var m in movements)
            {
                var amountInCRC = await _exchangeRateService.GetAmountInCRCAsync(m.Amount, m.Currency);
                if (m.Type == MovementType.Inbound)
                    totalIncomeInCRC += amountInCRC;
                else if (m.Type == MovementType.Outbound)
                    totalExpenseInCRC += amountInCRC;
            }

            var currentBalance = budget.RemainingAmountInCRC;
            var ratio = budget.OriginalAmountInCRC == 0
                ? 0
                : (totalExpenseInCRC / budget.OriginalAmountInCRC) * 100;

            var monthlyComparison = movements
                .GroupBy(m => new { m.Date.Year, m.Date.Month })
                .Select(g => new MonthlyComparisonDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Income = g.Where(m => m.Type == MovementType.Inbound)
                              .Sum(m => m.Amount),
                    Expense = g.Where(m => m.Type == MovementType.Outbound)
                               .Sum(m => m.Amount)
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToList();

            var dashboard = new FinancialDashboardDto
            {
                TotalIncome = totalIncomeInCRC,
                TotalExpense = totalExpenseInCRC,
                CurrentBudgetBalance = currentBalance,
                BudgetExecutionPercentage = ratio,
                MonthlyComparison = monthlyComparison
            };

            return Result<FinancialDashboardDto>.Success(dashboard);
        }

        public async Task<List<FinancialMovementDto>> GetMovementsByDateRangeAsync(DateTime from, DateTime to)
        {
            var movements = await _financialRepository.GetFinancialMovementsAsync(from, to);

            var result = movements.Select(m => new FinancialMovementDto
            {
                Id = m.Id,
                Description = m.Description,
                Amount = m.Amount,
                Currency = m.Currency,
                Date = m.Date,
                Type = m.Type,
                CreatedById = m.CreatedById,
                CreatedByName = m.CreatedBy?.NombreCompleto ?? "Desconocido"
            }).ToList();

            return result;
        }

        public async Task<List<BudgetDto>> GetAllBudgetsAsync()
        {
            var budgets = await _financialRepository.GetAllBudgetsAsync();
            var result = budgets.Select(b => new BudgetDto
            {
                Id = b.Id,
                OriginalAmount = b.Amount,
                Currency = b.Currency,
                StartDate = b.StartDate,
                EndDate = b.EndDate
            }).ToList();

            return result;
        }

        public async Task<Result> AddBudgetAsync(AddBudgetDto dto)
        {
            var budget = new Budget
            {
                Amount = dto.Amount,
                Currency = dto.Currency,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };

            await _financialRepository.AddBudgetAsync(budget);
            return Result.Success();
        }
    }
}

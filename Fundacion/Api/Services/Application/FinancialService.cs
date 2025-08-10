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

            var amountInCRC =await _exchangeRateService.GetAmountInCRCAsync(expenseDto.Amount, expenseDto.Currency);
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
    }
}

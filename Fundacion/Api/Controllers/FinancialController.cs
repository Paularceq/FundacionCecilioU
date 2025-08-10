using Api.Abstractions.Application;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.Financial;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinancialController : ControllerBase
    {
        private readonly IFinancialService _financialService;

        public FinancialController(IFinancialService financialService)
        {
            _financialService = financialService;
        }

        [HttpPost("lease-income")]
        public async Task<IActionResult> AddLeaseIncome([FromBody] LeaseIncomeDto leaseIncomeDto)
        {
            var result = await _financialService.AddLeaseIncomeAsync(leaseIncomeDto);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }
            return Ok(result);
        }

        [HttpPost("expense")]
        public async Task<IActionResult> AddExpense([FromBody] AdministrativeExpenseDto expenseDto)
        {
            var result = await _financialService.AddExpenseAsync(expenseDto);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }
            return Ok(result);
        }
    }
}

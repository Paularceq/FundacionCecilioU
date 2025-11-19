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
        private readonly IScholarshipPaymentService _scholarshipPaymentService;

        public FinancialController(
            IFinancialService financialService,
            IScholarshipPaymentService scholarshipPaymentService)
        {
            _financialService = financialService;
            _scholarshipPaymentService = scholarshipPaymentService;
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

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardData()
        {
            var dashboardData = await _financialService.GetDashboardAsync();
            if (dashboardData.IsFailure)
            {
                return BadRequest(dashboardData.Errors);
            }
            return Ok(dashboardData.Value);
        }

        [HttpGet("movements")]
        public async Task<IActionResult> GetFinancialMovements([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            if (!from.HasValue || !to.HasValue)
            {
                return BadRequest("Ambos parámetros de consulta startDate y endDate son requeridos.");
            }

            var movementsResult = await _financialService.GetMovementsByDateRangeAsync(from.Value, to.Value);

            return Ok(movementsResult);
        }

        [HttpGet("scholarships")]
        public async Task<IActionResult> GetScholarshipsWithPaymentStatus()
        {
            var scholarships = await _scholarshipPaymentService.GetScholarshipsWithPaymentStatusAsync();
            return Ok(scholarships);
        }

        [HttpPost("scholarships/process-pending")]
        public async Task<IActionResult> ProcessPendingScholarships([FromQuery] int userId)
        {
            var result = await _scholarshipPaymentService.ProcessPendingScholarshipsAsync(userId);
            if (result.IsFailure)
                return BadRequest(result.Errors);
            return Ok(result.Value);
        }

        [HttpPost("scholarships/process-payment")]
        public async Task<IActionResult> ProcessScholarshipPayment([FromQuery] int scholarshipId, [FromQuery] int userId)
        {
            var result = await _scholarshipPaymentService.ProcessScholarshipPaymentAsync(scholarshipId, userId);
            if (result.IsFailure)
                return BadRequest(result.Errors);
            return NoContent();
        }

        [HttpPost("scholarships/set-active-status")]
        public async Task<IActionResult> SetScholarshipActiveStatus([FromQuery] int scholarshipId, [FromQuery] bool isActive)
        {
            var result = await _scholarshipPaymentService.SetScholarshipActiveStatusAsync(scholarshipId, isActive);
            if (result.IsFailure)
                return BadRequest(result.Errors);
            return NoContent();
        }

        [HttpGet("budgets")]
        public async Task<IActionResult> GetAllBudgets()
        {
            var budgets = await _financialService.GetAllBudgetsAsync();
            return Ok(budgets);
        }

        [HttpPost("budgets")]
        public async Task<IActionResult> AddBudget([FromBody] AddBudgetDto dto)
        {
            var result = await _financialService.AddBudgetAsync(dto);
            if (result.IsFailure)
                return BadRequest(result.Errors);
            return Ok();
        }
    }
}

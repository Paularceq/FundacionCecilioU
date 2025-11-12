using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Constants;
using Shared.Dtos.Financial;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Extensions;
using Web.Models.Financial;
using Web.Services;

namespace Web.Controllers
{
    [Authorize(Roles = $"{Roles.AdminSistema},{Roles.AdminFinanzas}")]
    public class FinancialController : Controller
    {
        private readonly FinancialService _financialService;

        public FinancialController(FinancialService financialService)
        {
            _financialService = financialService;
        }

        public async Task<IActionResult> Index()
        {
            var dashboardData = await _financialService.GetDashboardAsync();
            if (dashboardData.IsFailure)
            {
                this.SetErrorMessage(dashboardData.Errors);
                return View();
            }

            return View(dashboardData.Value);
        }

        [HttpGet]
        public IActionResult AddLeaseIncome()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddLeaseIncome(LeaseIncomeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _financialService.AddLeaseIncomeAsync(userId, model);
            if (!result.IsSuccess)
            {
                this.SetErrorMessage(result.Errors);
                return View(model);
            }

            this.SetSuccessMessage("Ingreso por alquiler agregado exitosamente.");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult AddAdministrativeExpense()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddAdministrativeExpense(AdministrativeExpenseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _financialService.AddExpenseAsync(userId, model);
            if (!result.IsSuccess)
            {
                this.SetErrorMessage(result.Errors);
                return View(model);
            }

            this.SetSuccessMessage("Gasto administrativo agregado exitosamente.");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Movements()
        {
            return View(new FinancialMovementsFilterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Movements(FinancialMovementsFilterViewModel filter)
        {
            if (!ModelState.IsValid)
            {
                return View(filter);
            }

            var result = await _financialService.GetMovementsByDateRangeAsync(filter.From, filter.To);
            filter.Movements = result.Value;

            return View(filter);
        }

        [HttpGet]
        public async Task<IActionResult> Scholarships()
        {
            var result = await _financialService.GetScholarshipsWithPaymentStatusAsync();
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                return View(new List<ScholarshipWithPaymentStatusDto>());
            }
            return View(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPendingScholarships()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _financialService.ProcessPendingScholarshipsAsync(userId);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
            }
            else
            {
                this.SetSuccessMessage(result.Value);
            }
            return RedirectToAction("Scholarships");
        }

        [HttpPost]
        public async Task<IActionResult> ProcessScholarshipPayment(int scholarshipId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _financialService.ProcessScholarshipPaymentAsync(scholarshipId, userId);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
            }
            else
            {
                this.SetSuccessMessage("Se procesó el pago correctamente.");
            }
            return RedirectToAction("Scholarships");
        }

        [HttpPost]
        public async Task<IActionResult> SetScholarshipActiveStatus(int scholarshipId, bool isActive)
        {
            var result = await _financialService.SetScholarshipActiveStatusAsync(scholarshipId, isActive);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
            }
            else
            {
                this.SetSuccessMessage(isActive ? "Beca activada correctamente." : "Beca inactivada correctamente.");
            }
            return RedirectToAction("Scholarships");
        }

        [HttpGet]
        public async Task<IActionResult> Budgets()
        {
            var result = await _financialService.GetAllBudgetsAsync();
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                return View(new List<BudgetDto>());
            }
            return View(result.Value);
        }

        [HttpGet]
        public IActionResult AddBudget()
        {
            return View(new AddBudgetViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> AddBudget(AddBudgetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _financialService.AddBudgetAsync(model);
            if (!result.IsSuccess)
            {
                this.SetErrorMessage(result.Errors);
                return View(model);
            }

            this.SetSuccessMessage("Presupuesto agregado exitosamente.");
            return RedirectToAction("Budgets");
        }
    }
}

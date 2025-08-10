using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.Extensions;
using Web.Models.Financial;
using Web.Services;

namespace Web.Controllers
{
    public class FinancialController : Controller
    {
        private readonly FinancialService _financialService;

        public FinancialController(FinancialService financialService)
        {
            _financialService = financialService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
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
    }
}

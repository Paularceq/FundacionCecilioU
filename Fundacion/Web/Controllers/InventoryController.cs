using Microsoft.AspNetCore.Mvc;
using Shared.Dtos;
using Shared.Enums;
using System.Threading.Tasks;
using Web.Extensions;
using Web.Helpers;
using Web.Models.Inventory;
using Web.Services;

namespace Web.Controllers
{
    public class InventoryController : Controller
    {
        private readonly InventoryService _inventoryService;

        public InventoryController(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = await _inventoryService.GetAllProductsAsync();

            return View(products.Value);
        }

        [HttpGet]
        public IActionResult RegisterProduct()
        {
            var model = new RegisterProductViewModel
            {
                UnitsOfMeasure = EnumHelper.ToSelectListItems<UnitOfMeasure>()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterProduct(RegisterProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.UnitsOfMeasure = EnumHelper.ToSelectListItems<UnitOfMeasure>();
                return View(model);
            }

            var result = await _inventoryService.RegisterProductAsync(model);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                model.UnitsOfMeasure = EnumHelper.ToSelectListItems<UnitOfMeasure>();
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

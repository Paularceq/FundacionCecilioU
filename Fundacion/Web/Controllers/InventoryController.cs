using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Constants;
using Shared.Enums;
using Web.Extensions;
using Web.Helpers;
using Web.Models.Inventory;
using Web.Services;

namespace Web.Controllers
{
    [Authorize(Roles = Roles.AdminInventario)]
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
        public async Task<IActionResult> ProductDetails(int id)
        {
            var result = await _inventoryService.GetProductWithMovementsByIdAsync(id);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                return RedirectToAction(nameof(Index));
            }
            return View(result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var result = await _inventoryService.GetProductByIdAsync(id);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                return RedirectToAction(nameof(Index));
            }

            var product = result.Value;

            var model = new EditProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                MinimumStock = product.MinimumStock,
                SelectedUnitOfMeasure = product.UnitOfMeasure,
                UnitsOfMeasure = EnumHelper.ToSelectListItems<UnitOfMeasure>()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(EditProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.UnitsOfMeasure = EnumHelper.ToSelectListItems<UnitOfMeasure>();
                return View(model);
            }

            var result = await _inventoryService.UpdateProductAsync(model);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                model.UnitsOfMeasure = EnumHelper.ToSelectListItems<UnitOfMeasure>();
                return View(model);
            }

            return RedirectToAction(nameof(Index));
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

        [HttpGet]
        public async Task<IActionResult> AddStock(int id)
        {
            var result = await _inventoryService.GetProductByIdAsync(id);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                return RedirectToAction(nameof(Index));
            }

            var product = result.Value;
            var model = new InventoryMovementViewModel
            {
                ProductId = id,
                ProductName = product.Name,
                UnitOfMeasure = product.UnitOfMeasure,
                MinimumStock = product.MinimumStock,
                CurrentStock = product.CurrentStock,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddStock(InventoryMovementViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _inventoryService.AddStockAsync(model);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> WithdrawStock(int id)
        {
            var result = await _inventoryService.GetProductByIdAsync(id);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                return RedirectToAction(nameof(Index));
            }

            var product = result.Value;
            var model = new InventoryMovementViewModel
            {
                ProductId = id,
                ProductName = product.Name,
                UnitOfMeasure = product.UnitOfMeasure,
                MinimumStock = product.MinimumStock,
                CurrentStock = product.CurrentStock,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> WithdrawStock(InventoryMovementViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _inventoryService.WithdrawStockAsync(model);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

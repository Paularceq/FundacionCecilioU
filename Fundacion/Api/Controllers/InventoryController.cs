using Api.Abstractions.Application;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos;
using Shared.Models;

namespace Api.Controllers
{
    [Route("api/inventory")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpPost("add-stock")]
        public async Task<IActionResult> AddStock(InventoryMovementDto dto)
        {
            var result = await _inventoryService.AddStockAsync(dto);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }

        [HttpPost("withdraw-stock")]
        public async Task<IActionResult> WithdrawStock(InventoryMovementDto dto)
        {
            var result = await _inventoryService.WithdrawStockAsync(dto);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }

        [HttpGet("products")]
        public async Task<ActionResult<Result>> GetAllProducts()
        {
            var products = await _inventoryService.GetAllProductsAsync();

            return Ok(products);
        }

        [HttpPost("register-product")]
        public async Task<IActionResult> RegisterProduct(RegisterProductDto dto)
        {
            var result = await _inventoryService.RegisterProductAsync(dto);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }
    }
}

using Api.Abstractions.Application;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos;
using Shared.Dtos.Inventory;
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

        [HttpGet("products")]
        public async Task<ActionResult<Result>> GetAllProducts()
        {
            var products = await _inventoryService.GetAllProductsAsync();

            return Ok(products);
        }

        [HttpGet("products/{id}")]
        public async Task<ActionResult<ProductWithStockDto>> GetProductById(int id)
        {
            var product = await _inventoryService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpGet("products/{id}/movements")]
        public async Task<ActionResult<ProductWithMovementsDto>> GetProductWithMovementsById(int id)
        {
            var product = await _inventoryService.GetProductWithMovementsByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
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

        [HttpPut("update-product")]
        public async Task<IActionResult> UpdateProduct(UpdateProductDto dto)
        {
            var result = await _inventoryService.UpdateProductAsync(dto);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }

        [HttpPost("add-stock")]
        public async Task<IActionResult> AddStock(InventoryTransactionDto dto)
        {
            var result = await _inventoryService.AddStockAsync(dto);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }

        [HttpPost("withdraw-stock")]
        public async Task<IActionResult> WithdrawStock(InventoryTransactionDto dto)
        {
            var result = await _inventoryService.WithdrawStockAsync(dto);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }
    }
}

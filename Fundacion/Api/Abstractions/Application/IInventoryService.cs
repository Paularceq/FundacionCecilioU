using Shared.Dtos;
using Shared.Dtos.Inventory;
using Shared.Models;

namespace Api.Abstractions.Application
{
    public interface IInventoryService
    {
        Task<Result> AddStockAsync(InventoryTransactionDto dto);
        Task<IEnumerable<ProductWithStockDto>> GetAllProductsAsync();
        Task<ProductWithStockDto> GetProductByIdAsync(int id);
        Task<ProductWithMovementsDto> GetProductWithMovementsByIdAsync(int id);
        Task<Result> RegisterProductAsync(RegisterProductDto dto);
        Task<Result> UpdateProductAsync(UpdateProductDto dto);
        Task<Result> WithdrawStockAsync(InventoryTransactionDto dto);
    }
}
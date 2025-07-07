using Shared.Dtos;
using Shared.Models;

namespace Api.Abstractions.Application
{
    public interface IInventoryService
    {
        Task<Result> AddStockAsync(InventoryMovementDto dto);
        Task<IEnumerable<ProductStockDto>> GetAllProductsAsync();
        Task<Result> RegisterProductAsync(RegisterProductDto dto);
        Task<Result> WithdrawStockAsync(InventoryMovementDto dto);
    }
}
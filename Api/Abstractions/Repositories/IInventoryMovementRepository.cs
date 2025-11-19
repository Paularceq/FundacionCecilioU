using Api.Database.Entities;

namespace Api.Abstractions.Repositories
{
    public interface IInventoryMovementRepository
    {
        Task<InventoryMovement> AddAsync(InventoryMovement movement);
        Task<IEnumerable<InventoryMovement>> GetByProductIdAsync(int productId);
    }
}
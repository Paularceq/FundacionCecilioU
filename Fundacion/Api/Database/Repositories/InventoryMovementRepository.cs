using Api.Abstractions.Repositories;
using Api.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Database.Repositories
{
    public class InventoryMovementRepository : IInventoryMovementRepository
    {
        private readonly DatabaseContext _context;

        public InventoryMovementRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<InventoryMovement> AddAsync(InventoryMovement movement)
        {
            _context.InventoryMovements.Add(movement);
            await _context.SaveChangesAsync();
            return movement;
        }

        public async Task<IEnumerable<InventoryMovement>> GetByProductIdAsync(int productId)
        {
            return await _context.InventoryMovements
                .Where(m => m.ProductId == productId)
                .OrderByDescending(m => m.Date)
                .ToListAsync();
        }
    }
}

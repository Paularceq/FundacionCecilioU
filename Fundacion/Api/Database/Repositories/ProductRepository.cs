using Api.Abstractions.Repositories;
using Api.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Dtos.Inventory;
using Shared.Enums;

namespace Api.Database.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DatabaseContext _context;

        public ProductRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Movements)
                .SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<ProductWithStockDto>> GetAllWithStockAsync()
        {
            // Consulta todos los productos e incluye sus movimientos de inventario relacionados
            return await _context.Products
                .Include(p => p.Movements)
                .Select(p => new ProductWithStockDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    UnitOfMeasure = p.UnitOfMeasure,
                    MinimumStock = p.MinimumStock,
                    // Calcula el stock actual sumando las entradas y restando las salidas
                    CurrentStock = p.Movements
                        .Sum(m => m.Type == MovementType.Inbound ? m.Quantity : -m.Quantity),
                    // Determina si el stock actual es igual o menor al mínimo permitido
                    IsLowStock = p.Movements
                        .Sum(m => m.Type == MovementType.Inbound ? m.Quantity : -m.Quantity) <= p.MinimumStock
                })
                .ToListAsync();
        }

        public async Task<ProductWithStockDto> GetWithStockByIdAsync(int id)
        {
            // Consulta todos los productos e incluye sus movimientos de inventario relacionados
            return await _context.Products
                .Include(p => p.Movements)
                .Select(p => new ProductWithStockDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    UnitOfMeasure = p.UnitOfMeasure,
                    MinimumStock = p.MinimumStock,
                    // Calcula el stock actual sumando las entradas y restando las salidas
                    CurrentStock = p.Movements
                        .Sum(m => m.Type == MovementType.Inbound ? m.Quantity : -m.Quantity),
                    // Determina si el stock actual es igual o menor al mínimo permitido
                    IsLowStock = p.Movements
                        .Sum(m => m.Type == MovementType.Inbound ? m.Quantity : -m.Quantity) <= p.MinimumStock
                })
                .SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product> GetByNameAndUnitAsync(string name, UnitOfMeasure unit)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p =>
                    p.Name.ToLower() == name.ToLower() &&
                    p.UnitOfMeasure == unit);
        }

        public async Task<Product> AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }
    }
}

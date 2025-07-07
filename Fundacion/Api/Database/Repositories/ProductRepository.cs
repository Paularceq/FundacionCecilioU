using Api.Abstractions.Repositories;
using Api.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Dtos;
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

        public async Task<IEnumerable<ProductStockDto>> GetAllWithStockAsync()
        {
            // Consulta todos los productos e incluye sus movimientos de inventario relacionados
            return await _context.Products
                .Include(p => p.Movements)
                .Select(p => new ProductStockDto
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

        public async Task<ProductStockDto> GetWithStockByIdAsync(int id)
        {
            // Consulta todos los productos e incluye sus movimientos de inventario relacionados
            return await _context.Products
                .Include(p => p.Movements)
                .Select(p => new ProductStockDto
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
    }
}

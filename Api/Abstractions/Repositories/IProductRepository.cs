using Api.Database.Entities;
using Shared.Dtos.Inventory;
using Shared.Enums;

namespace Api.Abstractions.Repositories
{
    public interface IProductRepository
    {
        Task<Product> AddAsync(Product product);
        Task<IEnumerable<ProductWithStockDto>> GetAllWithStockAsync();
        Task<Product> GetByIdAsync(int id);
        Task<Product> GetByNameAndUnitAsync(string name, UnitOfMeasure unit);
        Task<ProductWithStockDto> GetWithStockByIdAsync(int id);
        Task<Product> UpdateAsync(Product product);
    }
}
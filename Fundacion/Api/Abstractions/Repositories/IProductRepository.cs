using Api.Database.Entities;
using Shared.Dtos;
using Shared.Enums;

namespace Api.Abstractions.Repositories
{
    public interface IProductRepository
    {
        Task<Product> AddAsync(Product product);
        Task<IEnumerable<ProductStockDto>> GetAllWithStockAsync();
        Task<Product> GetByNameAndUnitAsync(string name, UnitOfMeasure unit);
        Task<ProductStockDto> GetWithStockByIdAsync(int id);
    }
}
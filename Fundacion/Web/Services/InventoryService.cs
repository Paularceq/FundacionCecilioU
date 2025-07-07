using Shared.Dtos;
using Shared.Models;
using Web.Http;
using Web.Models.Inventory;

namespace Web.Services
{
    public class InventoryService
    {
        private readonly ApiClient _apiClient;

        public InventoryService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<Result<IEnumerable<ProductStockDto>>> GetAllProductsAsync()
        {
            var result = await _apiClient.GetAsync<IEnumerable<ProductStockDto>>("inventory/products");
            if (result.IsFailure)
                return Result<IEnumerable<ProductStockDto>>.Failure(result.Errors);

            return Result<IEnumerable<ProductStockDto>>.Success(result.Value);
        }

        public async Task<Result> RegisterProductAsync(RegisterProductViewModel model)
        {
            var dto = new RegisterProductDto
            {
                Name = model.Name,
                UnitOfMeasure = model.SelectedUnitOfMeasure,
                MinimumStock = model.MinimumStock,
                InitialQuantity = model.InitialQuantity,
                Comment = model.Comment
            };

            var result = await _apiClient.PostAsync("inventory/register-product", dto);
            if (result.IsFailure)
                return Result.Failure(result.Errors);

            return Result.Success();
        }
    }
}

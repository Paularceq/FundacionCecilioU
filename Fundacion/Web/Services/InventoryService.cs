using Shared.Dtos;
using Shared.Dtos.Inventory;
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

        public async Task<Result<IEnumerable<ProductWithStockDto>>> GetAllProductsAsync()
        {
            var result = await _apiClient.GetAsync<IEnumerable<ProductWithStockDto>>("inventory/products");
            if (result.IsFailure)
                return Result<IEnumerable<ProductWithStockDto>>.Failure(result.Errors);

            return Result<IEnumerable<ProductWithStockDto>>.Success(result.Value);
        }

        public async Task<Result<ProductWithStockDto>> GetProductByIdAsync(int id)
        {
            var result = await _apiClient.GetAsync<ProductWithStockDto>($"inventory/products/{id}");
            if (result.IsFailure)
                return Result<ProductWithStockDto>.Failure(result.Errors);

            return Result<ProductWithStockDto>.Success(result.Value);
        }

        public async Task<Result<ProductWithMovementsDto>> GetProductWithMovementsByIdAsync(int id)
        {
            var result = await _apiClient.GetAsync<ProductWithMovementsDto>($"inventory/products/{id}/movements");
            if (result.IsFailure)
                return Result<ProductWithMovementsDto>.Failure(result.Errors);
            return Result<ProductWithMovementsDto>.Success(result.Value);
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

        public async Task<Result> UpdateProductAsync(EditProductViewModel model)
        {
            var dto = new UpdateProductDto
            {
                Id = model.Id,
                Name = model.Name,
                UnitOfMeasure = model.SelectedUnitOfMeasure,
                MinimumStock = model.MinimumStock
            };

            var result = await _apiClient.PutAsync("inventory/update-product", dto);
            if (result.IsFailure)
                return Result.Failure(result.Errors);

            return Result.Success();
        }

        public async Task<Result> AddStockAsync(InventoryMovementViewModel model)
        {
            var dto = new InventoryTransactionDto
            {
                ProductId = model.ProductId,
                Quantity = model.Quantity,
                Comment = model.Comment
            };

            var result = await _apiClient.PostAsync("inventory/add-stock", dto);
            if (result.IsFailure)
                return Result.Failure(result.Errors);

            return Result.Success();
        }

        public async Task<Result> WithdrawStockAsync(InventoryMovementViewModel model)
        {
            var dto = new InventoryTransactionDto
            {
                ProductId = model.ProductId,
                Quantity = model.Quantity,
                Comment = model.Comment
            };

            var result = await _apiClient.PostAsync("inventory/withdraw-stock", dto);
            if (result.IsFailure)
                return Result.Failure(result.Errors);

            return Result.Success();
        }
    }
}

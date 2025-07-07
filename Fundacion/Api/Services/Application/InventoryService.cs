using Api.Abstractions.Application;
using Api.Abstractions.Repositories;
using Api.Database.Entities;
using Shared.Dtos;
using Shared.Enums;
using Shared.Models;

namespace Api.Services.Application
{
    public class InventoryService : IInventoryService
    {
        private readonly IProductRepository _productRepository;
        private readonly IInventoryMovementRepository _inventoryMovementRepository;

        public InventoryService(
            IProductRepository productRepository,
            IInventoryMovementRepository inventoryMovementRepository)
        {
            _productRepository = productRepository;
            _inventoryMovementRepository = inventoryMovementRepository;
        }

        public async Task<IEnumerable<ProductStockDto>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllWithStockAsync();
        }

        public async Task<Result> RegisterProductAsync(RegisterProductDto dto)
        {
            var existingProduct = await _productRepository
                .GetByNameAndUnitAsync(dto.Name, dto.UnitOfMeasure.Value);

            int productId;

            if (existingProduct == null)
            {
                var newProduct = new Product
                {
                    Name = dto.Name,
                    UnitOfMeasure = dto.UnitOfMeasure.Value,
                    MinimumStock = dto.MinimumStock
                };

                await _productRepository.AddAsync(newProduct);
                productId = newProduct.Id;
            }
            else
            {
                productId = existingProduct.Id;
            }

            var movement = new InventoryMovement
            {
                ProductId = productId,
                Quantity = dto.InitialQuantity,
                Type = MovementType.Inbound,
                Comment = string.IsNullOrWhiteSpace(dto.Comment) ? "Entrada de stock inicial." : dto.Comment
            };

            await _inventoryMovementRepository.AddAsync(movement);

            return Result.Success();
        }

        public async Task<Result> AddStockAsync(InventoryMovementDto dto)
        {
            var product = await _productRepository.GetWithStockByIdAsync(dto.ProductId);

            if (product == null)
                return Result.Failure("El producto especificado no existe en el inventario.");

            if (dto.Quantity <= 0)
                return Result.Failure("La cantidad a agregar debe ser mayor que cero.");

            var movement = new InventoryMovement
            {
                ProductId = product.Id,
                Quantity = dto.Quantity,
                Type = MovementType.Inbound,
                Comment = string.IsNullOrWhiteSpace(dto.Comment) ? "Stock added" : dto.Comment
            };

            await _inventoryMovementRepository.AddAsync(movement);

            return Result.Success();
        }

        public async Task<Result> WithdrawStockAsync(InventoryMovementDto dto)
        {
            var product = await _productRepository.GetWithStockByIdAsync(dto.ProductId);

            if (product == null)
                return Result.Failure("El producto especificado no existe en el inventario.");

            if (dto.Quantity <= 0)
                return Result.Failure("La cantidad a retirar debe ser mayor que cero.");

            if (dto.Quantity > product.CurrentStock)
                return Result.Failure("No hay suficiente stock disponible para realizar el retiro solicitado.");

            var movement = new InventoryMovement
            {
                ProductId = product.Id,
                Quantity = dto.Quantity,
                Type = MovementType.Outbound,
                Comment = string.IsNullOrWhiteSpace(dto.Comment) ? "Stock withdrawn" : dto.Comment
            };

            await _inventoryMovementRepository.AddAsync(movement);

            return Result.Success();
        }
    }
}

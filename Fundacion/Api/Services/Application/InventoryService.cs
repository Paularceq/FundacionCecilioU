using Api.Abstractions.Application;
using Api.Abstractions.Infrastructure;
using Api.Abstractions.Repositories;
using Api.Database.Entities;
using Shared.Constants;
using Shared.Dtos;
using Shared.Dtos.Inventory;
using Shared.Enums;
using Shared.Models;

namespace Api.Services.Application
{
    public class InventoryService : IInventoryService
    {
        private readonly IProductRepository _productRepository;
        private readonly IInventoryMovementRepository _inventoryMovementRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateService;

        public InventoryService(
            IProductRepository productRepository,
            IInventoryMovementRepository inventoryMovementRepository,
            IUserRepository userRepository,
            IEmailService emailService,
            IEmailTemplateService emailTemplateService)
        {
            _productRepository = productRepository;
            _inventoryMovementRepository = inventoryMovementRepository;
            _userRepository = userRepository;
            _emailService = emailService;
            _emailTemplateService = emailTemplateService;
        }

        public async Task<IEnumerable<ProductWithStockDto>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllWithStockAsync();
        }

        public async Task<ProductWithStockDto> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetWithStockByIdAsync(id);
        }

        public async Task<ProductWithMovementsDto> GetProductWithMovementsByIdAsync(int id)
        {
            var product = await _productRepository.GetWithStockByIdAsync(id);
            if (product == null)
                return null;

            var movements = await _inventoryMovementRepository.GetByProductIdAsync(id);

            return new ProductWithMovementsDto
            {
                Id = product.Id,
                Name = product.Name,
                UnitOfMeasure = product.UnitOfMeasure,
                MinimumStock = product.MinimumStock,
                CurrentStock = product.CurrentStock,
                IsLowStock = product.IsLowStock,
                Movements = movements.Select(m => new InventoryMovementDto
                {
                    Id = m.Id,
                    Quantity = m.Quantity,
                    Type = m.Type,
                    Comment = m.Comment,
                    Date = m.Date
                }).ToList()
            };
        }

        public async Task<Result> RegisterProductAsync(RegisterProductDto dto)
        {
            var existingProduct = await _productRepository.GetByNameAndUnitAsync(dto.Name, dto.UnitOfMeasure.Value);
            if (existingProduct != null)
                return Result.Failure("Ya existe un producto con ese nombre y unidad de medida.");

            var newProduct = new Product
            {
                Name = dto.Name,
                UnitOfMeasure = dto.UnitOfMeasure.Value,
                MinimumStock = dto.MinimumStock
            };

            await _productRepository.AddAsync(newProduct);

            var movement = new InventoryMovement
            {
                ProductId = newProduct.Id,
                Quantity = dto.InitialQuantity,
                Type = MovementType.Inbound,
                Comment = string.IsNullOrWhiteSpace(dto.Comment) ? "Entrada de stock inicial." : dto.Comment,
                Status = RequestStatus.Approved
            };

            await _inventoryMovementRepository.AddAsync(movement);

            return Result.Success();
        }

        public async Task<Result> UpdateProductAsync(UpdateProductDto dto)
        {
            var product = await _productRepository.GetByIdAsync(dto.Id);
            if (product == null)
                return Result.Failure("El producto especificado no existe.");

            product.Name = dto.Name;
            product.UnitOfMeasure = dto.UnitOfMeasure.Value;
            product.MinimumStock = dto.MinimumStock;

            await _productRepository.UpdateAsync(product);

            return Result.Success();
        }

        public async Task<Result> AddStockAsync(InventoryTransactionDto dto)
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
                Comment = string.IsNullOrWhiteSpace(dto.Comment) ? "Stock added" : dto.Comment,
                Status = RequestStatus.Approved
            };

            await _inventoryMovementRepository.AddAsync(movement);

            return Result.Success();
        }

        public async Task<Result> WithdrawStockAsync(InventoryTransactionDto dto)
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
                Comment = string.IsNullOrWhiteSpace(dto.Comment) ? "Stock withdrawn" : dto.Comment,
                Status = RequestStatus.Approved
            };

            await _inventoryMovementRepository.AddAsync(movement);

            var currentStock = product.CurrentStock - dto.Quantity;

            await CheckAndNotifyLowStockAsync(product.Name, currentStock, product.MinimumStock);

            return Result.Success();
        }

        private async Task CheckAndNotifyLowStockAsync(string productName, decimal currentStock, decimal minimumStock)
        {
            if (currentStock > minimumStock)
                return;

            var usersToNotify = await _userRepository.GetUsersByRole(Roles.AdminInventario);

            if (!usersToNotify.Any())
                return;

            var subject = "Notificación de stock bajo";
            var message = @$"
                <p>Estimado equipo de inventario,</p>
                <p>El producto <strong>{productName}</strong> ha alcanzado un nivel de stock bajo.</p>
                <p>Stock actual: <strong>{currentStock}</strong>, Stock mínimo: <strong>{minimumStock}</strong>.</p>
                <p>Por favor, tome las medidas necesarias para reabastecer el inventario.</p>";

            var body = await _emailTemplateService.RenderTemplateAsync(subject, subject, message);

            var sendEmailTasks = usersToNotify.Select(user => _emailService.SendEmailAsync(user.Email, subject, body));

            await Task.WhenAll(sendEmailTasks);
        }

    }
}

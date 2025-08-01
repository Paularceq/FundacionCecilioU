using Api.Abstractions.Application;
using Api.Abstractions.Infrastructure;
using Api.Abstractions.Repositories;
using Api.Database.Entities;
using Shared.Constants;
using Shared.Dtos.OutgoingDonations;
using Shared.Enums;
using Shared.Models;

namespace Api.Services.Application
{
    public class OutgoingDonationService : IOutgoingDonationService
    {
        private readonly IOutgoingDonationRepository _outgoingDonationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateService;

        public OutgoingDonationService(
            IOutgoingDonationRepository outgoingDonationRepository,
            IUserRepository userRepository,
            IProductRepository productRepository,
            IEmailService emailService,
            IEmailTemplateService emailTemplateService)
        {
            _outgoingDonationRepository = outgoingDonationRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _emailService = emailService;
            _emailTemplateService = emailTemplateService;
        }

        public async Task<IEnumerable<OutgoingDonationToListDto>> GetAllAsync()
        {
            var donations = await _outgoingDonationRepository.GetAllAsync();
            return donations.Select(d => new OutgoingDonationToListDto
            {
                Id = d.Id,
                RequesterName = d.Requester.NombreCompleto,
                RecipientName = d.Recipient.NombreCompleto,
                ApproverName = d.Approver?.NombreCompleto,
                RequestDate = d.RequestDate,
                ApprovalDate = d.ApprovalDate,
                Type = d.Type,
                Status = d.Status,
            });
        }

        public async Task<IEnumerable<OutgoingDonationToListDto>> GetAllByRequesterIdAsync(int requesterId)
        {
            var donations = await _outgoingDonationRepository.GetAllByRequesterIdAsync(requesterId);
            return donations.Select(d => new OutgoingDonationToListDto
            {
                Id = d.Id,
                RequesterName = d.Requester.NombreCompleto,
                RecipientName = d.Recipient.NombreCompleto,
                ApproverName = d.Approver?.NombreCompleto,
                RequestDate = d.RequestDate,
                ApprovalDate = d.ApprovalDate,
                Type = d.Type,
                Status = d.Status,
            });
        }

        public async Task<OutgoingDonationDetailsDto> GetByIdAsync(int id)
        {
            var donation = await _outgoingDonationRepository.GetByIdAsync(id);
            if (donation == null)
            {
                return null;
            }

            var details = new OutgoingDonationDetailsDto
            {
                Id = donation.Id,
                RequesterName = donation.Requester.NombreCompleto,
                RecipientName = donation.Recipient.NombreCompleto,
                ApproverName = donation.Approver?.NombreCompleto,
                RequestDate = donation.RequestDate,
                ApprovalDate = donation.ApprovalDate,
                Type = donation.Type,
                Status = donation.Status,
            };

            foreach (var item in donation.InventoryMovements)
            {
                var product = await _productRepository.GetWithStockByIdAsync(item.ProductId);
                if (product != null)
                {
                    details.Products.Add(new InKindProductDto
                    {
                        ProductId = product.Id,
                        Name = product.Name,
                        UnitOfMeasure = product.UnitOfMeasure,
                        RequestedQuantity = item.Quantity,
                        CurrentStock = product.CurrentStock
                    });
                }
            }

            return details;
        }

        public async Task<Result> CreateInKindDonationsAsync(CreateInKindDonationDto donationDto)
        {
            var validationResult = await ValidateProductsAsync(donationDto.Products);

            if (!validationResult.IsSuccess)
            {
                return Result.Failure(validationResult.Errors);
            }

            var outgoingDonation = new OutgoingDonation
            {
                RequesterId = donationDto.RequesterId,
                RecipientId = donationDto.RecipientId,
                RequestDate = DateTime.UtcNow,
                Type = DonationType.InKind,
                Status = RequestStatus.Pending,
                InventoryMovements = donationDto.Products.Select(p => new InventoryMovement
                {
                    ProductId = p.Id,
                    Quantity = p.Quantity,
                    Type = MovementType.Outbound,
                    Comment = "Donación en especie.",
                    Date = DateTime.UtcNow,
                    Status = RequestStatus.Pending,
                }).ToList(),
            };

            await _outgoingDonationRepository.AddAsync(outgoingDonation);

            await NotifyApproversAsync();

            return Result.Success();
        }

        public async Task<Result> ResolveInKindDonationAsync(ResolveOutgoingDonationRequestDto dto)
        {
            var donation = await _outgoingDonationRepository.GetByIdAsync(dto.DonationId);
            if (donation == null)
            {
                return Result.Failure("La donación no existe.");
            }
            if (donation.Status != RequestStatus.Pending)
            {
                return Result.Failure("La donación ya ha sido procesada.");
            }

            // Actualizar la donación con los datos de aprobación
            donation.ApproverId = dto.ApproverId;
            donation.ApprovalDate = DateTime.UtcNow;
            donation.Status = dto.IsApproved ? RequestStatus.Approved : RequestStatus.Rejected;

            // Validar los movimientos de inventario
            var validationResult = await ValidateProductsAsync(donation.InventoryMovements.Select( m => new InKindItemDto
            {
                Id = m.ProductId,
                Quantity = m.Quantity
            }));

            if (validationResult.IsFailure)
            {
                return Result.Failure(validationResult.Errors);
            }

            // Actualizar el estado de los movimientos de inventario
            foreach (var movement in donation.InventoryMovements)
            {
                movement.Status = dto.IsApproved ? RequestStatus.Approved : RequestStatus.Rejected;
            }

            // Actualizar el stock de los productos si la donación fue aprobada
            await _outgoingDonationRepository.UpdateAsync(donation);

            return Result.Success();
        }

        private async Task<Result> ValidateProductsAsync(IEnumerable<InKindItemDto> products)
        {
            if (products == null || !products.Any())
            {
                return Result.Failure("La donación debe contener al menos un producto.");
            }

            var errores = new List<string>();

            foreach (var product in products)
            {
                var existingProduct = await _productRepository.GetWithStockByIdAsync(product.Id);
                if (existingProduct == null)
                {
                    errores.Add($"El producto con ID {product.Id} no existe en el inventario.");
                    continue;
                }
                if (product.Quantity <= 0)
                {
                    errores.Add($"La cantidad a retirar del producto '{existingProduct.Name}' debe ser mayor que cero.");
                    continue;
                }
                if (product.Quantity > existingProduct.CurrentStock)
                {
                    errores.Add($"No hay suficiente stock disponible para el producto '{existingProduct.Name}'.");
                }
            }

            return new Result(errores);
        }

        private async Task NotifyApproversAsync()
        {
            var approvers = await _userRepository.GetUsersByRole(Roles.AdminSistema);

            if (approvers == null || !approvers.Any())
            {
                return; // No hay administradores para notificar
            }

            var subject = "Solicitud de donación saliente";
            var message = @$"
                <p>Estimado administrador,</p>
                <p>Se ha registrado una nueva solicitud de donación en especie.</p>
                <p>Por favor, revise y gestione la solicitud en el sistema.</p>";

            var body = await _emailTemplateService.RenderTemplateAsync(subject, subject, message);

            var sendEmailTasks = approvers.Select(user => _emailService.SendEmailAsync(user.Email, subject, body));

            await Task.WhenAll(sendEmailTasks);
        }
    }
}

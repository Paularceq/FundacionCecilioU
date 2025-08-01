using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.Constants;
using Shared.Dtos.OutgoingDonations;
using Shared.Extensions;
using Web.Extensions;
using Web.Models.OutgoingDonation;
using Web.Services;

namespace Web.Controllers
{
    public class OutgoingDonationController : Controller
    {
        private readonly OutgoingDonationService _outgoingDonationService;
        private readonly UserManagementService _userManagementService;
        private readonly InventoryService _inventoryService;

        public OutgoingDonationController(
            OutgoingDonationService outgoingDonationService,
            UserManagementService userManagementService,
            InventoryService inventoryService)
        {
            _outgoingDonationService = outgoingDonationService;
            _userManagementService = userManagementService;
            _inventoryService = inventoryService;
        }

        [HttpGet]
        [Authorize(Roles = Roles.AdminDonaciones)]
        public async Task<IActionResult> Index()
        {
            var requesterId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var donations = await _outgoingDonationService.GetAllByRequesterIdAsync(requesterId);

            return View(donations.Value);
        }

        [HttpGet]
        [Authorize(Roles = Roles.AdminSistema)]
        public async Task<IActionResult> Admin()
        {
            var donations = await _outgoingDonationService.GetAllAsync();
            return View(donations.Value);
        }

        [HttpGet]
        [Authorize(Roles = $"{Roles.AdminDonaciones},{Roles.AdminSistema}")]
        public async Task<IActionResult> Details(int id)
        {
            var donation = await _outgoingDonationService.GetByIdAsync(id);
            if (donation == null)
            {
                return NotFound();
            }
            return View(donation.Value);
        }

        [HttpGet]
        [Authorize(Roles = Roles.AdminDonaciones)]
        public async Task<IActionResult> Create()
        {
            var model = new CreateOutgoingDonationViewModel
            {
                SelectedProducts = [new InKindItemDto()]
            };
            await PopulateSelectListsAsync(model);
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = Roles.AdminDonaciones)]
        public async Task<IActionResult> Create(CreateOutgoingDonationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateSelectListsAsync(model);
                return View(model);
            }

            var dto = new CreateInKindDonationDto
            {
                RequesterId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value),
                RecipientId = model.SelectedStudentId,
                Products = model.SelectedProducts.ToList()
            };

            var result = await _outgoingDonationService.CreateInKindDonationsAsync(dto);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                await PopulateSelectListsAsync(model);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = Roles.AdminSistema)]
        public async Task<IActionResult> Resolve(int id, bool isApproved)
        {
            var dto = new ResolveOutgoingDonationRequestDto
            {
                DonationId = id,
                ApproverId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value),
                IsApproved = isApproved,
            };

            var result = await _outgoingDonationService.ResolveInKindDonationAsync(dto);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                return RedirectToAction(nameof(Admin));
            }

            this.SetSuccessMessage(isApproved ? "Donación aprobada exitosamente." : "Donación rechazada exitosamente.");

            return RedirectToAction(nameof(Admin));
        }

        private async Task PopulateSelectListsAsync(CreateOutgoingDonationViewModel model)
        {
            var students = await _userManagementService.GetAllByRoleAsync(Roles.Estudiante);
            var products = await _inventoryService.GetAllProductsAsync();

            model.Students = students.Value.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = $"{s.NombreCompleto} - {s.Identificacion}"
            });

            model.Products = products.Value.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Name} - {p.UnitOfMeasure.GetDescription()}"
            });
        }
    }
}

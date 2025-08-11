using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using System.Security.Claims;
using Web.Extensions;
using Web.Helpers;
using Web.Models.Donation;
using Web.Services;

namespace Web.Controllers
{
    public class DonationController : Controller
    {

        private readonly DonationService _donationService;

        public DonationController(DonationService donationService)
        {
            _donationService = donationService;
        }

        public async Task<IActionResult> IndexAsync()
        {
            // invocar el service para consultar las donaciones y pasarlo al view. Crear la vista con el nuevo dto como modelo
            var donations = await _donationService.GetAllDonationsAsync();
            return View(donations.Value);
        }
        [HttpGet]
        public IActionResult AddMonetaryDonation()
        {
            var model = new AddMonetaryDonationViewModel
            {
                AvailableCurrencies = EnumHelper.ToSelectListItems<Currency>()

            };
            return View(model);
        }

        [HttpPost]

        public async Task<IActionResult> AddMonetaryDonationAsync(AddMonetaryDonationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _donationService.AddMonetaryDonationAsync(userId, model);

            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                return View(model);
            }
            this.SetSuccessMessage("Se agregó la donación correctamente");
            return RedirectToAction("index", "dashboard");
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _donationService.GetDonationDetails(id);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                return RedirectToAction("index");
            }
            return View(result.Value);


        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
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

        public IActionResult Index()
        {
            // invocar el service para consultar las donaciones y pasarlo al view. Crear la vista con el nuevo dto como modelo
            return View();
        }
        [HttpGet]
        public IActionResult AddMonetaryDonation()
        {
            var model = new AddMonetaryDonationViewModel {
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

            var result = await _donationService.AddMonetaryDonationAsync(model);

            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                return View(model);
            }
            this.SetSuccessMessage("Se agregó la donación correctamente"); 
                return RedirectToAction("index", "dashboard");
        }

        
    }
}

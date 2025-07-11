using System.Security.Cryptography.X509Certificates;
using Shared.Dtos.Donations;
using Shared.Models;
using Web.Http;
using Web.Models.Donation;

namespace Web.Services
{
    public class DonationService
    {
        private readonly ApiClient _apiClient;

        public DonationService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<Result> AddMonetaryDonationAsync(AddMonetaryDonationViewModel addMonetaryDonationViewModel)
        {
            var dto = new AddMonetaryDonationDto
            {

                Amount = addMonetaryDonationViewModel.Amount,

                Currency = addMonetaryDonationViewModel.SelectedCurrency,

                Identification = addMonetaryDonationViewModel.Identification,

                Name = addMonetaryDonationViewModel.Name,
            };

            var result = await _apiClient.PostAsync("Donation/Add-MonetaryDonation",dto);

            if (result.IsFailure)
            {
                return Result.Failure(result.Errors);
            }
            return Result.Success();
        }

        // crear metodo que haga el request al Api con HttpGet
    }
}

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

        public async Task<Result> AddMonetaryDonationAsync(int userId, AddMonetaryDonationViewModel addMonetaryDonationViewModel)
        {
            var dto = new AddMonetaryDonationDto
            {
                Amount = addMonetaryDonationViewModel.Amount,
                Currency = addMonetaryDonationViewModel.SelectedCurrency,
                Identification = addMonetaryDonationViewModel.Identification,
                Name = addMonetaryDonationViewModel.Name,
                CreatedById = userId
            };

            var result = await _apiClient.PostAsync("Donation/Add-MonetaryDonation", dto);

            if (result.IsFailure)
            {
                return Result.Failure(result.Errors);
            }

            return Result.Success();
        }

        public async Task<Result<IEnumerable<DonationDto>>> GetAllDonationsAsync()
        {
            var result = await _apiClient.GetAsync<IEnumerable<DonationDto>>("Donation");
            if (result.IsFailure)
            {
                return Result<IEnumerable<DonationDto>>.Failure(result.Errors);
            }

            return Result<IEnumerable<DonationDto>>.Success(result.Value);
        }

        public async Task<Result<DonationDto>> GetDonationDetails(int id)
        {
            var result = await _apiClient.GetAsync<DonationDto>($"Donation/{id}");
            if (result.IsFailure)
            {
                return Result<DonationDto>.Failure(result.Errors);
            }
            return Result<DonationDto>.Success(result.Value);
        }
    }
}

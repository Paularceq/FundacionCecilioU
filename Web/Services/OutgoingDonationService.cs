using Shared.Dtos.OutgoingDonations;
using Shared.Models;
using Web.Http;

namespace Web.Services
{
    public class OutgoingDonationService
    {
        private readonly ApiClient _apiClient;

        public OutgoingDonationService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<Result<IEnumerable<OutgoingDonationToListDto>>> GetAllAsync()
        {
            var result = await _apiClient.GetAsync<IEnumerable<OutgoingDonationToListDto>>("OutgoingDonation");
            if (result.IsFailure)
                return Result<IEnumerable<OutgoingDonationToListDto>>.Failure(result.Errors);
            return Result<IEnumerable<OutgoingDonationToListDto>>.Success(result.Value);
        }

        public async Task<Result<IEnumerable<OutgoingDonationToListDto>>> GetAllByRequesterIdAsync(int requesterId)
        {
            var result = await _apiClient.GetAsync<IEnumerable<OutgoingDonationToListDto>>($"OutgoingDonation/requester/{requesterId}");
            if (result.IsFailure)
                return Result<IEnumerable<OutgoingDonationToListDto>>.Failure(result.Errors);

            return Result<IEnumerable<OutgoingDonationToListDto>>.Success(result.Value);
        }

        public async Task<Result<OutgoingDonationDetailsDto>> GetByIdAsync(int id)
        {
            var result = await _apiClient.GetAsync<OutgoingDonationDetailsDto>($"OutgoingDonation/{id}");
            if (result.IsFailure)
                return Result<OutgoingDonationDetailsDto>.Failure(result.Errors);
            return Result<OutgoingDonationDetailsDto>.Success(result.Value);
        }

        public async Task<Result> CreateInKindDonationsAsync(CreateInKindDonationDto donationDto)
        {
            var result = await _apiClient.PostAsync("OutgoingDonation/in-kind-donation", donationDto);
            if (result.IsFailure)
                return Result.Failure(result.Errors);

            return Result.Success();
        }

        public async Task<Result> ResolveInKindDonationAsync(ResolveOutgoingDonationRequestDto resolveDto)
        {
            var result = await _apiClient.PostAsync("OutgoingDonation/in-kind-donation/resolve", resolveDto);
            if (result.IsFailure)
                return Result.Failure(result.Errors);

            return Result.Success();
        }
    }
}

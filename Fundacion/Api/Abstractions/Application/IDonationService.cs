using Shared.Dtos.Donations;
using Shared.Models;

namespace Api.Abstractions.Application
{
    public interface IDonationService
    {
        Task<Result> AddMonetaryDonationAsync(AddMonetaryDonationDto dto);
        Task<IEnumerable<DonationDto>> GetAllDonationsAsync();
        Task<Result<DonationDto>> GetDonationDetails(int id);
    }
}
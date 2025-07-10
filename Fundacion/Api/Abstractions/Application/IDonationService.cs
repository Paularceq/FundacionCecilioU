using Shared.Dtos.Donations;
using Shared.Models;

namespace Api.Abstractions.Application
{
    public interface IDonationService
    {
        Task<Result> AddMonetaryDonationAsync(AddMonetaryDonationDto dto);
    }
}
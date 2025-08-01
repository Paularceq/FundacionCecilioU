using Shared.Dtos.OutgoingDonations;
using Shared.Models;

namespace Api.Abstractions.Application
{
    public interface IOutgoingDonationService
    {
        Task<Result> CreateInKindDonationsAsync(CreateInKindDonationDto donationDto);
        Task<IEnumerable<OutgoingDonationToListDto>> GetAllAsync();
        Task<IEnumerable<OutgoingDonationToListDto>> GetAllByRequesterIdAsync(int requesterId);
        Task<OutgoingDonationDetailsDto> GetByIdAsync(int id);
        Task<Result> ResolveInKindDonationAsync(ResolveOutgoingDonationRequestDto dto);
    }
}
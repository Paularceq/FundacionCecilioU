using Api.Database.Entities;

namespace Api.Abstractions.Repositories
{
    public interface IOutgoingDonationRepository
    {
        Task<OutgoingDonation> AddAsync(OutgoingDonation donation);
        Task<IEnumerable<OutgoingDonation>> GetAllAsync();
        Task<IEnumerable<OutgoingDonation>> GetAllByRequesterIdAsync(int requesterId);
        Task<OutgoingDonation> GetByIdAsync(int id);
        Task<IEnumerable<InventoryMovement>> GetInventoryMovements(int donationId);
        Task<OutgoingDonation> UpdateAsync(OutgoingDonation donation);
    }
}
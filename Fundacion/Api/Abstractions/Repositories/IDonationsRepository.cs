using Api.Database.Entities;

namespace Api.Abstractions.Repositories
{
    public interface IDonationsRepository
    {
        Task AddDonationAsync(Donation donation);
        Task AddDonationMonetary(MonetaryDonation monetarydonation);
        Task<IEnumerable<Donation>> GetAllDonationsAsync();
        Task<Donation> GetDonationById(int id);
        Task<MonetaryDonation> GetMonetaryDonation(int donationid);
        Task<IEnumerable<object>> GetMonetaryDonationsAsync();
    }
}
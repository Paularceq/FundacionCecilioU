using Api.Database.Entities;

namespace Api.Abstractions.Repositories
{
    public interface IDonationsRepository
    {
        Task AddDonationAsync(Donation donation);
        Task AddDonationMonetary(MonetaryDonation monetarydonation);
    }
}
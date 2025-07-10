using Api.Abstractions.Repositories;
using Api.Database.Entities;

namespace Api.Database.Repositories
{
    public class DonationsRepository : IDonationsRepository
    {

        private readonly DatabaseContext _context;

        public DonationsRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task AddDonationAsync(Donation donation)
        {
            _context.Donations.Add(donation);
            await _context.SaveChangesAsync();
        }

        public async Task AddDonationMonetary(MonetaryDonation monetarydonation)
        {
            _context.MonetaryDonations.Add(monetarydonation);
            await _context.SaveChangesAsync();
        }
    }
}

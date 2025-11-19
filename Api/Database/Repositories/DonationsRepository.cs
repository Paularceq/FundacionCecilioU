using Api.Abstractions.Repositories;
using Api.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Dtos.Donations;
using System.Threading.Tasks;

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

        public Task<IEnumerable<object>> GetMonetaryDonationsAsync()
        {
            throw new NotImplementedException();
        }

        //crear metodo que consulta todas las donaciones

        public async Task<IEnumerable <Donation>> GetAllDonationsAsync()
        {

            return await _context.Donations.ToListAsync();

        }

        public async Task<Donation> GetDonationById(int id)
        {
            return await _context.Donations.FindAsync(id);
        }

        public async Task<MonetaryDonation> GetMonetaryDonation(int donationid)
        {
            return await _context.MonetaryDonations.SingleOrDefaultAsync(d => d.DonationId == donationid);

        }

    }
}

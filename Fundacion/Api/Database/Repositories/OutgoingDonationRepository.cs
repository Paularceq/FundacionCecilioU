using Api.Abstractions.Repositories;
using Api.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;

namespace Api.Database.Repositories
{
    public class OutgoingDonationRepository : IOutgoingDonationRepository
    {
        private readonly DatabaseContext _context;

        public OutgoingDonationRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<OutgoingDonation> AddAsync(OutgoingDonation donation)
        {
            _context.OutgoingDonations.Add(donation);
            await _context.SaveChangesAsync();
            return donation;
        }

        public async Task<OutgoingDonation> UpdateAsync(OutgoingDonation donation)
        {
            _context.OutgoingDonations.Update(donation);
            await _context.SaveChangesAsync();
            return donation;
        }

        public async Task<IEnumerable<OutgoingDonation>> GetAllAsync()
        {
            return await _context.OutgoingDonations
                .Include(d => d.Requester)
                .Include(d => d.Recipient)
                .Include(d => d.Approver)
                .ToListAsync();
        }

        public async Task<IEnumerable<OutgoingDonation>> GetAllByRequesterIdAsync(int requesterId)
        {
            return await _context.OutgoingDonations
                .Include(d => d.Requester)
                .Include(d => d.Recipient)
                .Include(d => d.Approver)
                .Where(d => d.RequesterId == requesterId)
                .ToListAsync();
        }

        public async Task<OutgoingDonation> GetByIdAsync(int id)
        {
            return await _context.OutgoingDonations
                .Include(d => d.Requester)
                .Include(d => d.Recipient)
                .Include(d => d.Approver)
                .Include(d => d.InventoryMovements)
                .ThenInclude(m => m.Product)
                .SingleOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<InventoryMovement>> GetInventoryMovements(int donationId)
        {
            var donation = await _context.OutgoingDonations
                .Include(d => d.InventoryMovements)
                .SingleOrDefaultAsync(d => d.Id == donationId);

            return donation?.InventoryMovements ?? [];
        }
    }
}

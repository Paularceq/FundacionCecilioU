using Api.Abstractions.Repositories;
using Api.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Database.Repositories
{
    public class VolunteerRequestRepository : IVolunteerRequestRepository
    {
        private readonly DatabaseContext _context;

        public VolunteerRequestRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<List<VolunteerRequest>> GetRequestsByVolunteerID(int volunteerID)
        {
            return await _context.VolunteerRequests
                .Include(v=>v.Volunteer)
                .Include(v=>v.Approver)
                .Where(v => v.VolunteerId == volunteerID).ToListAsync();
        }
        public async Task CreateRequest(VolunteerRequest voluteerRequest)
        {
            _context.VolunteerRequests.Add(voluteerRequest);
            await _context.SaveChangesAsync();
        }
        public async Task <VolunteerRequest> GetActiveRequest(int VolunteerId)
        {
            return await _context.VolunteerRequests
                .Where(v => v.VolunteerId == VolunteerId && (v.State == Shared.Enums.VolunteerState.Pending  || v.State == Shared.Enums.VolunteerState.Approved))
                .FirstOrDefaultAsync();
        }
    }
}

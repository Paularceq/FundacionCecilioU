using Api.Abstractions.Repositories;
using Api.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Database.Repositories
{
    public class HomeContentRepository : IHomeContentRepository
    {
        private readonly DatabaseContext _context;

        public HomeContentRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HomeContent>> GetAllHomeContentAsync()
        {
            return await _context.HomeContents
                .Include(hc => hc.Creator)
                .OrderByDescending(hc => hc.CreatedDate)
                .ToListAsync();
        }

        public async Task<HomeContent?> GetActiveHomeContentAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.HomeContents
                .Include(hc => hc.Creator)
                .FirstOrDefaultAsync(hc => hc.IsActive &&
                    (hc.StartDate == null || hc.StartDate <= now) &&
                    (hc.EndDate == null || hc.EndDate >= now));
        }

        public async Task<HomeContent?> GetHomeContentByIdAsync(int id)
        {
            return await _context.HomeContents
                .Include(hc => hc.Creator)
                .FirstOrDefaultAsync(hc => hc.Id == id);
        }

        public async Task<HomeContent> CreateHomeContentAsync(HomeContent content)
        {
            _context.HomeContents.Add(content);
            await _context.SaveChangesAsync();
            return content;
        }

        public async Task<HomeContent> UpdateHomeContentAsync(HomeContent content)
        {
            _context.HomeContents.Update(content);
            await _context.SaveChangesAsync();
            return content;
        }

        public async Task DeleteHomeContentAsync(int id)
        {
            var content = await _context.HomeContents.FindAsync(id);
            if (content != null)
            {
                _context.HomeContents.Remove(content);
                await _context.SaveChangesAsync();
            }
        }
    }
}

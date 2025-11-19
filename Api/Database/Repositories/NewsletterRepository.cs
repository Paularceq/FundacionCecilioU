using Api.Abstractions.Repositories;
using Api.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Database.Repositories
{
    public class NewsletterRepository : INewsletterRepository
    {
        private readonly DatabaseContext _context;

        public NewsletterRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Newsletter>> GetAllNewslettersAsync()
        {
            return await _context.Newsletters
                .Include(n => n.Creator)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task<Newsletter?> GetNewsletterByIdAsync(int id)
        {
            return await _context.Newsletters
                .Include(n => n.Creator)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<Newsletter> CreateNewsletterAsync(Newsletter newsletter)
        {
            _context.Newsletters.Add(newsletter);
            await _context.SaveChangesAsync();
            return newsletter;
        }

        public async Task<Newsletter> UpdateNewsletterAsync(Newsletter newsletter)
        {
            _context.Newsletters.Update(newsletter);
            await _context.SaveChangesAsync();
            return newsletter;
        }

        public async Task DeleteNewsletterAsync(int id)
        {
            var newsletter = await _context.Newsletters.FindAsync(id);
            if (newsletter != null)
            {
                _context.Newsletters.Remove(newsletter);
                await _context.SaveChangesAsync();
            }
        }
    }
}
using Api.Abstractions.Repositories;
using Api.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;

namespace Api.Database.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly DatabaseContext _context;

        public SubscriptionRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NewsletterSubscription>> GetAllSubscriptionsAsync()
        {
            return await _context.NewsletterSubscriptions
                .OrderByDescending(ns => ns.SubscriptionDate)
                .ToListAsync();
        }

        public async Task<NewsletterSubscription?> GetSubscriptionByIdAsync(int id)
        {
            return await _context.NewsletterSubscriptions
                .FirstOrDefaultAsync(ns => ns.Id == id);
        }

        public async Task<NewsletterSubscription?> GetSubscriptionByEmailAsync(string email)
        {
            return await _context.NewsletterSubscriptions
                .FirstOrDefaultAsync(ns => ns.Email == email);
        }

        public async Task<NewsletterSubscription?> GetSubscriptionByTokenAsync(string token)
        {
            return await _context.NewsletterSubscriptions
                .FirstOrDefaultAsync(ns => ns.ConfirmationToken == token);
        }

        public async Task<NewsletterSubscription> CreateSubscriptionAsync(NewsletterSubscription subscription)
        {
            _context.NewsletterSubscriptions.Add(subscription);
            await _context.SaveChangesAsync();
            return subscription;
        }

        public async Task<NewsletterSubscription> UpdateSubscriptionAsync(NewsletterSubscription subscription)
        {
            _context.NewsletterSubscriptions.Update(subscription);
            await _context.SaveChangesAsync();
            return subscription;
        }

        public async Task<IEnumerable<NewsletterSubscription>> GetActiveSubscriptionsAsync()
        {
            return await _context.NewsletterSubscriptions
                .Where(ns => ns.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<NewsletterSubscription>> GetSubscriptionsByFrequencyAsync(SubscriptionFrequency frequency)
        {
            return await _context.NewsletterSubscriptions
                .Where(ns => ns.IsActive && ns.Frequency == frequency)
                .ToListAsync();
        }

        public async Task<int> GetActiveSubscriptionsCountAsync()
        {
            return await _context.NewsletterSubscriptions
                .CountAsync(ns => ns.IsActive);
        }
    }
}
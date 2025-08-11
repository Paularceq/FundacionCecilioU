using Api.Database.Entities;
using Shared.Enums;

namespace Api.Abstractions.Repositories
{
    public interface ISubscriptionRepository
    {
        Task<IEnumerable<NewsletterSubscription>> GetAllSubscriptionsAsync();
        Task<NewsletterSubscription?> GetSubscriptionByIdAsync(int id);
        Task<NewsletterSubscription?> GetSubscriptionByEmailAsync(string email);
        Task<NewsletterSubscription?> GetSubscriptionByTokenAsync(string token);
        Task<NewsletterSubscription> CreateSubscriptionAsync(NewsletterSubscription subscription);
        Task<NewsletterSubscription> UpdateSubscriptionAsync(NewsletterSubscription subscription);
        Task<IEnumerable<NewsletterSubscription>> GetActiveSubscriptionsAsync();
        Task<IEnumerable<NewsletterSubscription>> GetSubscriptionsByFrequencyAsync(SubscriptionFrequency frequency);
        Task<int> GetActiveSubscriptionsCountAsync();
    }
using Shared.Dtos.PublicContent;
using Shared.Models;

namespace Api.Abstractions.Application
{
    public interface ISubscriptionService
    {
        Task<Result<IEnumerable<SubscriptionToListDto>>> GetAllSubscriptionsAsync();
        Task<Result<SubscriptionDto>> GetSubscriptionByIdAsync(int id);
        Task<Result> CreateSubscriptionAsync(CreateSubscriptionDto subscriptionDto);
        Task<Result> ConfirmSubscriptionAsync(string token);
        Task<Result> UnsubscribeAsync(string email);
        Task<Result> ToggleSubscriptionStatusAsync(int id);
        Task<Result<int>> GetActiveSubscriptionsCountAsync();
        Task<Result<IEnumerable<SubscriptionToListDto>>> GetSubscriptionsByFrequencyAsync(int frequency);
    }
}
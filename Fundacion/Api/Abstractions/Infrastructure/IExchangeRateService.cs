using Shared.Enums;

namespace Api.Abstractions.Infrastructure
{
    public interface IExchangeRateService
    {
        Task<decimal> GetAmountInCRCAsync(decimal amount, Currency currency);
        Task<decimal> GetExchangeRateForCRCAsync(Currency currency);
    }
}
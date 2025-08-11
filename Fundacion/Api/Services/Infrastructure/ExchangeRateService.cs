using Api.Abstractions.Infrastructure;
using Shared.Enums;

namespace Api.Services.Infrastructure
{
    public class ExchangeRateService : IExchangeRateService
    {
        public async Task<decimal> GetExchangeRateForCRCAsync(Currency currency)
        {
            decimal rate = currency switch
            {
                Currency.CRC => 1m,
                Currency.USD => 540m,
                Currency.EUR => 590m,
                _ => throw new ArgumentOutOfRangeException(nameof(currency), "Unsupported currency")
            };

            return await Task.FromResult(rate);
        }

        public async Task<decimal> GetAmountInCRCAsync(decimal amount, Currency currency)
        {
            var exchangeRate = await GetExchangeRateForCRCAsync(currency);
            return amount * exchangeRate;
        }
    }
}

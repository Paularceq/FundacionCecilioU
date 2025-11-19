using Api.Abstractions.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using Shared.Enums;
using System.Xml.Linq;

namespace Api.Services.Infrastructure
{
    public class BccrHttpExchangeRateService : IExchangeRateService
    {
        private readonly IMemoryCache _cache;
        private readonly IHttpClientFactory _httpClientFactory;
        private const string BccrUrl = "https://gee.bccr.fi.cr/Indicadores/Suscripciones/WS/wsindicadoreseconomicos.asmx/ObtenerIndicadoresEconomicos";
        private const string Email = "fabian30leon@gmail.com";
        private const string Token = "N8OFB554LA";

        public BccrHttpExchangeRateService(IMemoryCache cache, IHttpClientFactory httpClientFactory)
        {
            _cache = cache;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<decimal> GetAmountInCRCAsync(decimal amount, Currency currency)
        {
            var exchangeRate = await GetExchangeRateForCRCAsync(currency);
            return amount * exchangeRate;
        }

        public async Task<decimal> GetExchangeRateForCRCAsync(Currency currency)
        {
            if (currency == Currency.CRC)
                return 1m;

            if (currency == Currency.USD)
            {
                int usdIndicator = 317;
                string cacheKey = $"{usdIndicator}_{DateTime.Now:yyyyMMdd}";
                if (_cache.TryGetValue(cacheKey, out decimal cachedRate))
                    return cachedRate;

                var usdRate = await GetIndicatorRateAsync(usdIndicator);
                _cache.Set(cacheKey, usdRate, TimeSpan.FromHours(24));
                return usdRate;
            }

            if (currency == Currency.EUR)
            {
                int eurIndicator = 333;
                string eurCacheKey = $"{eurIndicator}_{DateTime.Now:yyyyMMdd}";
                string usdCacheKey = $"317_{DateTime.Now:yyyyMMdd}";

                if (!_cache.TryGetValue(eurCacheKey, out decimal eurToUsdRate))
                {
                    eurToUsdRate = await GetIndicatorRateAsync(eurIndicator);
                    _cache.Set(eurCacheKey, eurToUsdRate, TimeSpan.FromHours(24));
                }

                if (!_cache.TryGetValue(usdCacheKey, out decimal usdToCrcRate))
                {
                    usdToCrcRate = await GetIndicatorRateAsync(317);
                    _cache.Set(usdCacheKey, usdToCrcRate, TimeSpan.FromHours(24));
                }

                return eurToUsdRate * usdToCrcRate;
            }

            throw new NotSupportedException($"Currency {currency} is not supported.");
        }

        private async Task<decimal> GetIndicatorRateAsync(int indicador)
        {
            var today = DateTime.Now;
            var fecha = today.ToString("dd/MM/yyyy");
            var cacheKey = $"{indicador}_{today:yyyyMMdd}";

            if (_cache.TryGetValue(cacheKey, out decimal cachedRate))
                return cachedRate;

            var payload = new Dictionary<string, string>
            {
                { "FechaInicio", fecha },
                { "FechaFinal", fecha },
                { "Nombre", "N" },
                { "SubNiveles", "N" },
                { "Indicador", indicador.ToString() },
                { "CorreoElectronico", Email },
                { "Token", Token }
            };

            var client = _httpClientFactory.CreateClient();
            var content = new FormUrlEncodedContent(payload);

            var response = await client.PostAsync(BccrUrl, content);
            response.EnsureSuccessStatusCode();

            var xml = await response.Content.ReadAsStringAsync();
            var doc = XDocument.Parse(xml);
            var numValorElement = doc.Descendants("NUM_VALOR").FirstOrDefault();
            if (numValorElement == null)
                throw new InvalidOperationException("NUM_VALOR not found in response.");

            var rate = decimal.Parse(numValorElement.Value, System.Globalization.CultureInfo.InvariantCulture);
            _cache.Set(cacheKey, rate, TimeSpan.FromHours(24));
            return rate;
        }
    }
}
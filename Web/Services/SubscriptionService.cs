using Shared.Dtos.PublicContent;
using Shared.Models;
using Web.Http;
using Web.Models.Newsletter;

namespace Web.Services
{
    public class SubscriptionService
    {
        private readonly ApiClient _apiClient;

        public SubscriptionService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        // Métodos públicos
        public async Task<Result> CreateSubscriptionAsync(SubscribeViewModel model)
        {
            // 1. Convertir ViewModel a DTO
            var dto = new CreateSubscriptionDto
            {
                Email = model.Email,
                Name = model.Name,
                Frequency = model.Frequency
            };

            // 2. Enviar el request al backend
            var response = await _apiClient.PostAsync("Subscription/Subscribe", dto);

            // 3. Validar el resultado
            if (response.IsFailure)
                return Result.Failure(response.Errors);

            return Result.Success();
        }

        public async Task<Result> ConfirmSubscriptionAsync(string token)
        {
            // Nota: Este endpoint devuelve un wrapper, pero solo necesitamos saber si fue exitoso
            var response = await _apiClient.GetAsync<dynamic>($"Subscription/Confirm?token={token}");
            if (response.IsFailure)
                return Result.Failure(response.Errors);

            return Result.Success();
        }

        public async Task<Result> UnsubscribeAsync(string email)
        {
            var dto = new { Email = email };
            var response = await _apiClient.PostAsync("Subscription/Unsubscribe", dto);
            if (response.IsFailure)
                return Result.Failure(response.Errors);

            return Result.Success();
        }

        // Métodos administrativos
        public async Task<Result<IEnumerable<SubscriptionViewModel>>> GetAllSubscriptionsAsync()
        {
            var result = await _apiClient.GetAsync<IEnumerable<SubscriptionToListDto>>("Subscription/All");
            if (result.IsFailure)
                return Result<IEnumerable<SubscriptionViewModel>>.Failure(result.Errors);

            var viewModels = result.Value.Select(dto => new SubscriptionViewModel
            {
                Id = dto.Id,
                Email = dto.Email,
                Name = dto.Name,
                IsActive = dto.IsActive,
                SubscriptionDate = dto.SubscriptionDate,
                Frequency = dto.Frequency
            });

            return Result<IEnumerable<SubscriptionViewModel>>.Success(viewModels);
        }

        public async Task<Result<SubscriptionViewModel>> GetSubscriptionByIdAsync(int id)
        {
            var result = await _apiClient.GetAsync<SubscriptionDto>($"Subscription/{id}");
            if (result.IsFailure)
                return Result<SubscriptionViewModel>.Failure(result.Errors);

            var viewModel = new SubscriptionViewModel
            {
                Id = result.Value.Id,
                Email = result.Value.Email,
                Name = result.Value.Name,
                IsActive = result.Value.IsActive,
                SubscriptionDate = result.Value.SubscriptionDate,
                Frequency = result.Value.Frequency.ToString()
            };

            return Result<SubscriptionViewModel>.Success(viewModel);
        }

        public async Task<Result> ToggleSubscriptionStatusAsync(int id)
        {
            var response = await _apiClient.PostAsync($"Subscription/ToggleStatus/{id}");
            if (response.IsFailure)
                return Result.Failure(response.Errors);

            return Result.Success();
        }

        public async Task<Result<int>> GetActiveSubscriptionsCountAsync()
        {
            var result = await _apiClient.GetAsync<int>("Subscription/Count");
            if (result.IsFailure)
                return Result<int>.Failure(result.Errors);

            return Result<int>.Success(result.Value);
        }

        public async Task<Result<IEnumerable<SubscriptionViewModel>>> GetSubscriptionsByFrequencyAsync(int frequency)
        {
            var result = await _apiClient.GetAsync<IEnumerable<SubscriptionToListDto>>($"Subscription/ByFrequency/{frequency}");
            if (result.IsFailure)
                return Result<IEnumerable<SubscriptionViewModel>>.Failure(result.Errors);

            var viewModels = result.Value.Select(dto => new SubscriptionViewModel
            {
                Id = dto.Id,
                Email = dto.Email,
                Name = dto.Name,
                IsActive = dto.IsActive,
                SubscriptionDate = dto.SubscriptionDate,
                Frequency = dto.Frequency
            });

            return Result<IEnumerable<SubscriptionViewModel>>.Success(viewModels);
        }

        public async Task<Result<NewsletterStatsViewModel>> GetSubscriptionStatisticsAsync()
        {
            // Este endpoint mantiene wrapper para estadísticas
            var result = await _apiClient.GetAsync<dynamic>("Subscription/Statistics");
            if (result.IsFailure)
                return Result<NewsletterStatsViewModel>.Failure(result.Errors);

            try
            {
                var stats = new NewsletterStatsViewModel
                {
                    TotalSubscriptions = (int)result.Value.statistics.total,
                    ActiveSubscriptions = (int)result.Value.statistics.active,
                    InactiveSubscriptions = (int)result.Value.statistics.inactive,
                    DailySubscriptions = (int)result.Value.statistics.byFrequency.daily,
                    WeeklySubscriptions = (int)result.Value.statistics.byFrequency.weekly,
                    MonthlySubscriptions = (int)result.Value.statistics.byFrequency.monthly,
                    GrowthThisMonth = (int)result.Value.statistics.growthThisMonth
                };

                return Result<NewsletterStatsViewModel>.Success(stats);
            }
            catch
            {
                return Result<NewsletterStatsViewModel>.Failure("Error al procesar estadísticas");
            }
        }
    }
}
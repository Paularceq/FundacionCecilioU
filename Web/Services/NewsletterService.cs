using Shared.Dtos.PublicContent;
using Shared.Models;
using Web.Http;
using Web.Models.Newsletter;

namespace Web.Services
{
    public class NewsletterService
    {
        private readonly ApiClient _apiClient;

        public NewsletterService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<Result<IEnumerable<NewsletterViewModel>>> GetAllNewslettersAsync()
        {
            var result = await _apiClient.GetAsync<IEnumerable<NewsletterDto>>("Newsletter/All");
            if (result.IsFailure)
                return Result<IEnumerable<NewsletterViewModel>>.Failure(result.Errors);

            var viewModels = result.Value.Select(dto => new NewsletterViewModel
            {
                Id = dto.Id,
                Subject = dto.Subject,
                CustomContent = dto.CustomContent,
                SendDate = dto.SendDate,
                Status = dto.Status,
                RecipientCount = dto.RecipientCount,
                CreatedBy = dto.CreatedBy,
                CreatedDate = dto.CreatedDate
            });

            return Result<IEnumerable<NewsletterViewModel>>.Success(viewModels);
        }

        public async Task<Result<NewsletterViewModel>> GetNewsletterByIdAsync(int id)
        {
            var result = await _apiClient.GetAsync<NewsletterDto>($"Newsletter/{id}");
            if (result.IsFailure)
                return Result<NewsletterViewModel>.Failure(result.Errors);

            var viewModel = new NewsletterViewModel
            {
                Id = result.Value.Id,
                Subject = result.Value.Subject,
                CustomContent = result.Value.CustomContent,
                SendDate = result.Value.SendDate,
                Status = result.Value.Status,
                RecipientCount = result.Value.RecipientCount,
                CreatedBy = result.Value.CreatedBy,
                CreatedDate = result.Value.CreatedDate
            };

            return Result<NewsletterViewModel>.Success(viewModel);
        }

        public async Task<Result> CreateNewsletterAsync(CreateNewsletterViewModel model)
        {
            // 1. Convertir ViewModel a DTO
            var dto = new CreateNewsletterDto
            {
                Subject = model.Subject,
                CustomContent = model.CustomContent,
                SendDate = model.SendDate,
                IncludeHomeContent = model.IncludeHomeContent,
                SendNow = model.SendNow
            };

            // 2. Enviar el request al backend
            var response = await _apiClient.PostAsync("Newsletter/Create", dto);

            // 3. Validar el resultado
            if (response.IsFailure)
                return Result.Failure(response.Errors);

            return Result.Success();
        }

        public async Task<Result> SendNewsletterAsync(int id)
        {
            var response = await _apiClient.PostAsync($"Newsletter/Send/{id}");
            if (response.IsFailure)
                return Result.Failure(response.Errors);

            return Result.Success();
        }

        public async Task<Result> DeleteNewsletterAsync(int id)
        {
            var response = await _apiClient.DeleteAsync($"Newsletter/{id}");
            if (response.IsFailure)
                return Result.Failure(response.Errors);

            return Result.Success();
        }

        public async Task<Result<string>> GenerateNewsletterPreviewAsync(CreateNewsletterViewModel model)
        {
            // 1. Convertir ViewModel a DTO
            var dto = new CreateNewsletterDto
            {
                Subject = model.Subject,
                CustomContent = model.CustomContent,
                IncludeHomeContent = model.IncludeHomeContent
            };

            // 2. Enviar el request al backend
            var result = await _apiClient.PostAsync<CreateNewsletterDto, string>("Newsletter/Preview", dto);

            // 3. Validar el resultado
            if (result.IsFailure)
                return Result<string>.Failure(result.Errors);

            return Result<string>.Success(result.Value);
        }
    }
}
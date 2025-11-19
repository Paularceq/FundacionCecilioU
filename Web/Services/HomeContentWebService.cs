using Shared.Dtos.PublicContent;
using Shared.Models;
using Web.Http;
using Web.Models.Newsletter;

namespace Web.Services
{
    public class HomeContentService
    {
        private readonly ApiClient _apiClient;

        public HomeContentService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<Result<IEnumerable<HomeContentViewModel>>> GetAllHomeContentAsync()
        {
            var result = await _apiClient.GetAsync<IEnumerable<HomeContentDto>>("HomeContent/All");
            if (result.IsFailure)
                return Result<IEnumerable<HomeContentViewModel>>.Failure(result.Errors);

            var viewModels = result.Value.Select(dto => new HomeContentViewModel
            {
                Id = dto.Id,
                Title = dto.Title,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                IsActive = dto.IsActive,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                CreatedBy = dto.CreatedBy,
                CreatedDate = dto.CreatedDate
            });

            return Result<IEnumerable<HomeContentViewModel>>.Success(viewModels);
        }

        public async Task<Result<HomeContentViewModel>> GetActiveHomeContentAsync()
        {
            var result = await _apiClient.GetAsync<HomeContentDto>("HomeContent/Active");
            if (result.IsFailure)
                return Result<HomeContentViewModel>.Failure(result.Errors);

            var viewModel = new HomeContentViewModel
            {
                Id = result.Value.Id,
                Title = result.Value.Title,
                Description = result.Value.Description,
                ImageUrl = result.Value.ImageUrl,
                IsActive = result.Value.IsActive,
                StartDate = result.Value.StartDate,
                EndDate = result.Value.EndDate,
                CreatedBy = result.Value.CreatedBy,
                CreatedDate = result.Value.CreatedDate
            };

            return Result<HomeContentViewModel>.Success(viewModel);
        }

        public async Task<Result<HomeContentViewModel>> GetHomeContentByIdAsync(int id)
        {
            var result = await _apiClient.GetAsync<HomeContentDto>($"HomeContent/{id}");
            if (result.IsFailure)
                return Result<HomeContentViewModel>.Failure(result.Errors);

            var viewModel = new HomeContentViewModel
            {
                Id = result.Value.Id,
                Title = result.Value.Title,
                Description = result.Value.Description,
                ImageUrl = result.Value.ImageUrl,
                IsActive = result.Value.IsActive,
                StartDate = result.Value.StartDate,
                EndDate = result.Value.EndDate,
                CreatedBy = result.Value.CreatedBy,
                CreatedDate = result.Value.CreatedDate
            };

            return Result<HomeContentViewModel>.Success(viewModel);
        }

        public async Task<Result> CreateHomeContentAsync(CreateHomeContentViewModel model)
        {
            // 1. Convertir ViewModel a DTO
            var dto = new CreateHomeContentDto
            {
                Title = model.Title,
                Description = model.Description,
                ImageUrl = model.ImageUrl,
                IsActive = model.IsActive,
                StartDate = model.StartDate,
                EndDate = model.EndDate
            };

            // 2. Enviar el request al backend
            var response = await _apiClient.PostAsync("HomeContent/Create", dto);

            // 3. Validar el resultado
            if (response.IsFailure)
                return Result.Failure(response.Errors);

            return Result.Success();
        }

        public async Task<Result> UpdateHomeContentAsync(UpdateHomeContentViewModel model)
        {
            // 1. Convertir ViewModel a DTO
            var dto = new UpdateHomeContentDto
            {
                Id = model.Id,
                Title = model.Title,
                Description = model.Description,
                ImageUrl = model.ImageUrl,
                IsActive = model.IsActive,
                StartDate = model.StartDate,
                EndDate = model.EndDate
            };

            // 2. Enviar el request al backend
            var response = await _apiClient.PutAsync("HomeContent/Update", dto);

            // 3. Validar el resultado
            if (response.IsFailure)
                return Result.Failure(response.Errors);

            return Result.Success();
        }

        public async Task<Result> DeleteHomeContentAsync(int id)
        {
            var response = await _apiClient.DeleteAsync($"HomeContent/{id}");
            if (response.IsFailure)
                return Result.Failure(response.Errors);

            return Result.Success();
        }

        public async Task<Result> ToggleHomeContentStatusAsync(int id)
        {
            var response = await _apiClient.PostAsync($"HomeContent/ToggleStatus/{id}");
            if (response.IsFailure)
                return Result.Failure(response.Errors);

            return Result.Success();
        }
    }
}
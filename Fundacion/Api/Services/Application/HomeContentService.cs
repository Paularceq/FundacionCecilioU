using Api.Abstractions.Application;
using Api.Abstractions.Infrastructure;
using Api.Abstractions.Repositories;
using Api.Database.Entities;
using Shared.Dtos.PublicContent;
using Shared.Models;

namespace Api.Services.Application
{
    public class HomeContentService : IHomeContentService
    {
        private readonly IHomeContentRepository _homeContentRepository;
        private readonly IUserRepository _userRepository;

        public HomeContentService(IHomeContentRepository homeContentRepository, IUserRepository userRepository)
        {
            _homeContentRepository = homeContentRepository;
            _userRepository = userRepository;
        }

        public async Task<Result<IEnumerable<HomeContentDto>>> GetAllHomeContentAsync()
        {
            var contents = await _homeContentRepository.GetAllHomeContentAsync();
            var contentDtos = contents.Select(c => new HomeContentDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                IsActive = c.IsActive,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                CreatedBy = c.Creator?.NombreCompleto ?? "Usuario desconocido",
                CreatedDate = c.CreatedDate
            });

            return Result<IEnumerable<HomeContentDto>>.Success(contentDtos);
        }

        public async Task<Result<HomeContentDto>> GetActiveHomeContentAsync()
        {
            var content = await _homeContentRepository.GetActiveHomeContentAsync();
            if (content == null)
            {
                return Result<HomeContentDto>.Failure("No hay contenido activo disponible.");
            }

            var contentDto = new HomeContentDto
            {
                Id = content.Id,
                Title = content.Title,
                Description = content.Description,
                ImageUrl = content.ImageUrl,
                IsActive = content.IsActive,
                StartDate = content.StartDate,
                EndDate = content.EndDate,
                CreatedBy = content.Creator?.NombreCompleto ?? "Usuario desconocido",
                CreatedDate = content.CreatedDate
            };

            return Result<HomeContentDto>.Success(contentDto);
        }

        public async Task<Result<HomeContentDto>> GetHomeContentByIdAsync(int id)
        {
            var content = await _homeContentRepository.GetHomeContentByIdAsync(id);
            if (content == null)
            {
                return Result<HomeContentDto>.Failure("Contenido no encontrado.");
            }

            var contentDto = new HomeContentDto
            {
                Id = content.Id,
                Title = content.Title,
                Description = content.Description,
                ImageUrl = content.ImageUrl,
                IsActive = content.IsActive,
                StartDate = content.StartDate,
                EndDate = content.EndDate,
                CreatedBy = content.Creator?.NombreCompleto ?? "Usuario desconocido",
                CreatedDate = content.CreatedDate
            };

            return Result<HomeContentDto>.Success(contentDto);
        }

        public async Task<Result> CreateHomeContentAsync(CreateHomeContentDto contentDto, int userId)
        {
            // Validar fechas
            if (contentDto.StartDate.HasValue && contentDto.EndDate.HasValue &&
                contentDto.StartDate > contentDto.EndDate)
            {
                return Result.Failure("La fecha de inicio no puede ser posterior a la fecha de fin.");
            }

            // Verificar que el usuario existe
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return Result.Failure("Usuario no encontrado.");
            }

            var newContent = new HomeContent
            {
                Title = contentDto.Title,
                Description = contentDto.Description,
                ImageUrl = contentDto.ImageUrl,
                IsActive = contentDto.IsActive,
                StartDate = contentDto.StartDate,
                EndDate = contentDto.EndDate,
                CreatedBy = userId,
                CreatedDate = DateTime.UtcNow
            };

            await _homeContentRepository.CreateHomeContentAsync(newContent);
            return Result.Success();
        }

        public async Task<Result> UpdateHomeContentAsync(UpdateHomeContentDto contentDto)
        {
            var existingContent = await _homeContentRepository.GetHomeContentByIdAsync(contentDto.Id);
            if (existingContent == null)
            {
                return Result.Failure("Contenido no encontrado.");
            }

            // Validar fechas
            if (contentDto.StartDate.HasValue && contentDto.EndDate.HasValue &&
                contentDto.StartDate > contentDto.EndDate)
            {
                return Result.Failure("La fecha de inicio no puede ser posterior a la fecha de fin.");
            }

            existingContent.Title = contentDto.Title;
            existingContent.Description = contentDto.Description;
            existingContent.ImageUrl = contentDto.ImageUrl;
            existingContent.IsActive = contentDto.IsActive;
            existingContent.StartDate = contentDto.StartDate;
            existingContent.EndDate = contentDto.EndDate;

            await _homeContentRepository.UpdateHomeContentAsync(existingContent);
            return Result.Success();
        }

        public async Task<Result> DeleteHomeContentAsync(int id)
        {
            var content = await _homeContentRepository.GetHomeContentByIdAsync(id);
            if (content == null)
            {
                return Result.Failure("Contenido no encontrado.");
            }

            await _homeContentRepository.DeleteHomeContentAsync(id);
            return Result.Success();
        }

        public async Task<Result> ToggleHomeContentStatusAsync(int id)
        {
            var content = await _homeContentRepository.GetHomeContentByIdAsync(id);
            if (content == null)
            {
                return Result.Failure("Contenido no encontrado.");
            }

            content.IsActive = !content.IsActive;
            await _homeContentRepository.UpdateHomeContentAsync(content);
            return Result.Success();
        }
    }
}
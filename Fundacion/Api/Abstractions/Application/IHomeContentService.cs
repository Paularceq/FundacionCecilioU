using Shared.Dtos.PublicContent;
using Shared.Models;

namespace Api.Abstractions.Application
{
    public interface IHomeContentService
    {
        Task<Result<IEnumerable<HomeContentDto>>> GetAllHomeContentAsync();
        Task<Result<HomeContentDto>> GetActiveHomeContentAsync();
        Task<Result<HomeContentDto>> GetHomeContentByIdAsync(int id);
        Task<Result> CreateHomeContentAsync(CreateHomeContentDto contentDto, int userId);
        Task<Result> UpdateHomeContentAsync(UpdateHomeContentDto contentDto);
        Task<Result> DeleteHomeContentAsync(int id);
        Task<Result> ToggleHomeContentStatusAsync(int id);
    }
}
using Api.Database.Entities;

namespace Api.Abstractions.Repositories
{
    public interface IHomeContentRepository
    {
        Task<IEnumerable<HomeContent>> GetAllHomeContentAsync();
        Task<HomeContent?> GetActiveHomeContentAsync();
        Task<HomeContent?> GetHomeContentByIdAsync(int id);
        Task<HomeContent> CreateHomeContentAsync(HomeContent content);
        Task<HomeContent> UpdateHomeContentAsync(HomeContent content);
        Task DeleteHomeContentAsync(int id);
    }
}

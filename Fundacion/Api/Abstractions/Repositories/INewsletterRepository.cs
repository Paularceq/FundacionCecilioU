using Api.Database.Entities;

namespace Api.Abstractions.Repositories
{
    public interface INewsletterRepository
    {
        Task<IEnumerable<Newsletter>> GetAllNewslettersAsync();
        Task<Newsletter?> GetNewsletterByIdAsync(int id);
        Task<Newsletter> CreateNewsletterAsync(Newsletter newsletter);
        Task<Newsletter> UpdateNewsletterAsync(Newsletter newsletter);
        Task DeleteNewsletterAsync(int id);
    }
}
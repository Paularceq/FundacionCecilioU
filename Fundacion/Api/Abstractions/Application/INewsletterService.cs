using Shared.Dtos.PublicContent;
using Shared.Models;

namespace Api.Abstractions.Application
{
    public interface INewsletterService
    {
        Task<Result<IEnumerable<NewsletterDto>>> GetAllNewslettersAsync();
        Task<Result<NewsletterDto>> GetNewsletterByIdAsync(int id);
        Task<Result> CreateNewsletterAsync(CreateNewsletterDto newsletterDto, int userId);
        Task<Result> SendNewsletterAsync(int newsletterId);
        Task<Result> DeleteNewsletterAsync(int id);
        Task<Result<string>> GenerateNewsletterPreviewAsync(CreateNewsletterDto newsletterDto);
    }
}
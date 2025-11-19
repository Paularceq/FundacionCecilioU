namespace Api.Abstractions.Infrastructure
{
    public interface IEmailTemplateService
    {
        Task<string> RenderTemplateAsync(string subject, string header, string body);
    }
}
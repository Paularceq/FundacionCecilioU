using Api.Abstractions.Infrastructure;

namespace Api.Services.Infrastructure;

public class DummyEmailService : IEmailService
{
    public Task SendEmailAsync(string to, string subject, string body)
    {
        return Task.CompletedTask;
    }
}

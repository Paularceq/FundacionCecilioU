using Api.Abstractions.Infrastructure;
using System.Net;
using System.Net.Mail;

namespace Api.Services.Infrastructure
{
    public class SmtpEmailService : IEmailService
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;
        private readonly string _from;

        public SmtpEmailService(IConfiguration configuration)
        {
            _host = configuration["SmtpSettings:Host"];
            _port = int.Parse(configuration["SmtpSettings:Port"]);
            _username = configuration["SmtpSettings:Username"];
            _password = configuration["SmtpSettings:Password"];
            _from = configuration["SmtpSettings:From"];
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            using var client = new SmtpClient(_host, _port)
            {
                Credentials = new NetworkCredential(_username, _password),
                EnableSsl = true
            };

            var mailMessage = new MailMessage(_from, to, subject, body)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(mailMessage);
        }
    }
}

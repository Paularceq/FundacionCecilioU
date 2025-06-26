using Api.Abstractions.Infrastructure;

namespace Api.Services.Infrastructure
{
    public sealed class EmailTemplateService : IEmailTemplateService
    {
        private static readonly string Template = @"
<!DOCTYPE html>
<html lang=""es"">
<head>
    <meta charset=""UTF-8"">
    <title>{{Subject}}</title>
    <style>
        .container {
            max-width: 600px;
            margin: 0 auto;
            background: #fff;
            border-radius: 8px;
            border: 1px solid #e3e3e3;
            font-family: 'Segoe UI', Arial, sans-serif;
            padding: 32px 24px;
        }
        .header {
            background: #0d6efd;
            color: #fff;
            padding: 16px 0;
            text-align: center;
            border-radius: 8px 8px 0 0;
            font-size: 1.5rem;
            font-weight: bold;
        }
        .content {
            margin: 24px 0;
            color: #212529;
            font-size: 1rem;
        }
        .footer {
            text-align: center;
            color: #6c757d;
            font-size: 0.9rem;
            margin-top: 32px;
        }
    </style>
</head>
<body style=""background: #f8f9fa; margin:0; padding:24px;"">
    <div class=""container"">
        <div class=""header"">{{Header}}</div>
        <div class=""content"">{{Body}}</div>
        <div class=""footer"">© {{Year}} Tu Empresa. Todos los derechos reservados.</div> 
    </div>
</body>
</html>";

        public Task<string> RenderTemplateAsync(string subject, string header, string body)
        {
            var htmlContent = Template
                .Replace("{{Subject}}", subject)
                .Replace("{{Header}}", header)
                .Replace("{{Body}}", body)
                .Replace("{{Year}}", DateTime.UtcNow.Year.ToString());

            return Task.FromResult(htmlContent);
        }
    }
}

using Api.Abstractions.Infrastructure;
using Azure;
using Azure.Communication.Email;

namespace Api.Services.Infrastructure;

public class AzureEmailService : IEmailService
{
    private readonly EmailClient _emailClient;
    private readonly string _senderAddress;

    // Necesitarás una cadena de conexión o un endpoint/credenciales
    // y la dirección del remitente (MailFrom address) de tu dominio verificado.
    public AzureEmailService(IConfiguration configuration)
    {
        // Inicializa EmailClient con la cadena de conexión
        _emailClient = new EmailClient(configuration["AzureCommunication:ConnectionString"]);
        _senderAddress = configuration["AzureCommunication:SenderAddress"];
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var emailContent = new EmailContent(subject)
        {
            // Establecemos el cuerpo del correo como HTML (puedes usar Text si lo prefieres)
            Html = body
        };

        var emailRecipients = new EmailRecipients(
        [
            new EmailAddress(to)
        ]);

        var emailMessage = new EmailMessage(_senderAddress, emailRecipients, emailContent);

        try
        {
            // El método SendAsync encola el correo electrónico.
            // WaitUntil.Completed espera a que el servicio devuelva el estado final del envío.
            EmailSendOperation emailSendOperation = await _emailClient.SendAsync(
                WaitUntil.Completed,
                emailMessage);

            // Opcional: Puedes verificar el estado de la operación si lo necesitas
            if (emailSendOperation.HasCompleted && emailSendOperation.Value.Status != EmailSendStatus.Succeeded)
            {
                // Manejo de errores si el envío no fue exitoso después del polling
                // El OperationId puede ser útil para el troubleshooting en Azure.
                throw new Exception($"Email failed to send. Status: {emailSendOperation.Value.Status}, Operation ID: {emailSendOperation.Id}");
            }
        }
        catch (RequestFailedException ex)
        {
            // Manejo de errores a nivel de solicitud HTTP (ej. autenticación, throttling)
            Console.WriteLine($"Email send request failed: {ex.Message}");
            throw;
        }
    }
}
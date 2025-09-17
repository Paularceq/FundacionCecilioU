using Api.Abstractions.Application;
using Api.Abstractions.Infrastructure;
using Api.Abstractions.Repositories;
using Api.Database.Entities;
using Shared.Dtos.PublicContent;
using Shared.Enums;
using Shared.Models;

namespace Api.Services.Application
{
    public class NewsletterService : INewsletterService
    {
        private readonly INewsletterRepository _newsletterRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IHomeContentRepository _homeContentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        public NewsletterService(
            INewsletterRepository newsletterRepository,
            ISubscriptionRepository subscriptionRepository,
            IHomeContentRepository homeContentRepository,
            IUserRepository userRepository,
            IEmailService emailService)
        {
            _newsletterRepository = newsletterRepository;
            _subscriptionRepository = subscriptionRepository;
            _homeContentRepository = homeContentRepository;
            _userRepository = userRepository;
            _emailService = emailService;
        }

        public async Task<Result<IEnumerable<NewsletterDto>>> GetAllNewslettersAsync()
        {
            var newsletters = await _newsletterRepository.GetAllNewslettersAsync();
            var newsletterDtos = newsletters.Select(n => new NewsletterDto
            {
                Id = n.Id,
                Subject = n.Subject,
                CustomContent = n.CustomContent,
                SendDate = n.SendDate,
                Status = n.Status,
                RecipientCount = n.RecipientCount,
                CreatedBy = n.Creator?.NombreCompleto ?? "Usuario desconocido",
                CreatedDate = n.CreatedDate
            });

            return Result<IEnumerable<NewsletterDto>>.Success(newsletterDtos);
        }

        public async Task<Result<NewsletterDto>> GetNewsletterByIdAsync(int id)
        {
            var newsletter = await _newsletterRepository.GetNewsletterByIdAsync(id);
            if (newsletter == null)
            {
                return Result<NewsletterDto>.Failure("Newsletter no encontrado.");
            }

            var newsletterDto = new NewsletterDto
            {
                Id = newsletter.Id,
                Subject = newsletter.Subject,
                CustomContent = newsletter.CustomContent,
                SendDate = newsletter.SendDate,
                Status = newsletter.Status,
                RecipientCount = newsletter.RecipientCount,
                CreatedBy = newsletter.Creator?.NombreCompleto ?? "Usuario desconocido",
                CreatedDate = newsletter.CreatedDate
            };

            return Result<NewsletterDto>.Success(newsletterDto);
        }

        public async Task<Result> CreateNewsletterAsync(CreateNewsletterDto newsletterDto, int userId)
        {
            // Verificar que el usuario existe
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return Result.Failure("Usuario no encontrado.");
            }

            var activeSubscriptions = await _subscriptionRepository.GetActiveSubscriptionsAsync();
            var recipientCount = activeSubscriptions.Count();

            var newNewsletter = new Newsletter
            {
                Subject = newsletterDto.Subject,
                CustomContent = newsletterDto.CustomContent,
                SendDate = newsletterDto.SendNow ? DateTime.UtcNow : newsletterDto.SendDate,
                Status = newsletterDto.SendNow ? NewsletterStatus.Sending : NewsletterStatus.Draft,
                RecipientCount = recipientCount,
                CreatedBy = userId,
                CreatedDate = DateTime.UtcNow
            };

            await _newsletterRepository.CreateNewsletterAsync(newNewsletter);

            // Si se debe enviar ahora, enviar inmediatamente
            if (newsletterDto.SendNow)
            {
                await SendNewsletterAsync(newNewsletter.Id);
            }

            return Result.Success();
        }

        public async Task<Result> SendNewsletterAsync(int newsletterId)
        {
            var newsletter = await _newsletterRepository.GetNewsletterByIdAsync(newsletterId);
            if (newsletter == null)
            {
                return Result.Failure("Newsletter no encontrado.");
            }

            if (newsletter.Status == NewsletterStatus.Sent)
            {
                return Result.Failure("Este newsletter ya ha sido enviado.");
            }

            // Actualizar estado a enviando
            newsletter.Status = NewsletterStatus.Sending;
            newsletter.SendDate = DateTime.UtcNow;
            await _newsletterRepository.UpdateNewsletterAsync(newsletter);

            try
            {
                // Obtener suscriptores activos
                var activeSubscriptions = await _subscriptionRepository.GetActiveSubscriptionsAsync();

                // Generar contenido del email
                var emailContent = await GenerateEmailContentAsync(newsletter);

                // Enviar a cada suscriptor
                foreach (var subscription in activeSubscriptions)
                {
                    var personalizedContent = emailContent.Replace("[NOMBRE]", subscription.Name);
                    await _emailService.SendEmailAsync(subscription.Email, newsletter.Subject, personalizedContent);
                }

                // Actualizar estado a enviado
                newsletter.Status = NewsletterStatus.Sent;
                newsletter.RecipientCount = activeSubscriptions.Count();
                await _newsletterRepository.UpdateNewsletterAsync(newsletter);

                return Result.Success();
            }
            catch (Exception)
            {
                // Marcar como fallido en caso de error
                newsletter.Status = NewsletterStatus.Failed;
                await _newsletterRepository.UpdateNewsletterAsync(newsletter);
                return Result.Failure("Error al enviar el newsletter.");
            }
        }

        public async Task<Result> DeleteNewsletterAsync(int id)
        {
            var newsletter = await _newsletterRepository.GetNewsletterByIdAsync(id);
            if (newsletter == null)
            {
                return Result.Failure("Newsletter no encontrado.");
            }

            if (newsletter.Status == NewsletterStatus.Sent)
            {
                return Result.Failure("No se puede eliminar un newsletter que ya ha sido enviado.");
            }

            await _newsletterRepository.DeleteNewsletterAsync(id);
            return Result.Success();
        }

        public async Task<Result<string>> GenerateNewsletterPreviewAsync(CreateNewsletterDto newsletterDto)
        {
            var emailContent = await GenerateEmailContentAsync(newsletterDto);
            return Result<string>.Success(emailContent);
        }

        private async Task<string> GenerateEmailContentAsync(Newsletter newsletter)
        {
            return await GenerateEmailContentAsync(new CreateNewsletterDto
            {
                Subject = newsletter.Subject,
                CustomContent = newsletter.CustomContent,
                IncludeHomeContent = true
            });
        }

        private async Task<string> GenerateEmailContentAsync(CreateNewsletterDto newsletterDto)
        {
            var content = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h1 style='color: #2c3e50; text-align: center;'>{newsletterDto.Subject}</h1>
                        
                        <div style='margin: 20px 0;'>
                            {newsletterDto.CustomContent}
                        </div>";

            // Incluir contenido del home si está habilitado
            if (newsletterDto.IncludeHomeContent)
            {
                var homeContent = await _homeContentRepository.GetActiveHomeContentAsync();
                if (homeContent != null)
                {
                    content += $@"
                        <hr style='border: 1px solid #eee; margin: 30px 0;'>
                        <h2 style='color: #2c3e50;'>Nuestros Últimos Logros</h2>
                        <div style='background: #f8f9fa; padding: 20px; border-radius: 5px;'>
                            <h3>{homeContent.Title}</h3>
                            <p>{homeContent.Description}</p>
                            {(!string.IsNullOrEmpty(homeContent.ImageUrl) ? $"<img src='{homeContent.ImageUrl}' alt='Imagen' style='max-width: 100%; height: auto;'>" : "")}
                        </div>";
                }
            }

            content += @"
                        <hr style='border: 1px solid #eee; margin: 30px 0;'>
                        <p style='text-align: center; color: #666; font-size: 14px;'>
                            Gracias por su apoyo.<br>
                            <a href='#' style='color: #666;'>Darse de baja</a>
                        </p>
                    </div>
                </body>
                </html>";

            return content;
        }
    }
}
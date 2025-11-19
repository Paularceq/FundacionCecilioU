using Api.Abstractions.Application;
using Api.Abstractions.Infrastructure;
using Api.Abstractions.Repositories;
using Api.Database.Entities;
using Shared.Dtos.PublicContent;
using Shared.Enums;
using Shared.Models;

namespace Api.Services.Application
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IEmailService _emailService;

        public SubscriptionService(ISubscriptionRepository subscriptionRepository, IEmailService emailService)
        {
            _subscriptionRepository = subscriptionRepository;
            _emailService = emailService;
        }

        public async Task<Result<IEnumerable<SubscriptionToListDto>>> GetAllSubscriptionsAsync()
        {
            var subscriptions = await _subscriptionRepository.GetAllSubscriptionsAsync();
            var subscriptionDtos = subscriptions.Select(s => new SubscriptionToListDto
            {
                Id = s.Id,
                Email = s.Email,
                Name = s.Name,
                IsActive = s.IsActive,
                SubscriptionDate = s.SubscriptionDate,
                Frequency = s.Frequency.ToString()
            });

            return Result<IEnumerable<SubscriptionToListDto>>.Success(subscriptionDtos);
        }

        public async Task<Result<SubscriptionDto>> GetSubscriptionByIdAsync(int id)
        {
            var subscription = await _subscriptionRepository.GetSubscriptionByIdAsync(id);
            if (subscription == null)
            {
                return Result<SubscriptionDto>.Failure("Suscripción no encontrada.");
            }

            var subscriptionDto = new SubscriptionDto
            {
                Id = subscription.Id,
                Email = subscription.Email,
                Name = subscription.Name,
                IsActive = subscription.IsActive,
                SubscriptionDate = subscription.SubscriptionDate,
                Frequency = subscription.Frequency
            };

            return Result<SubscriptionDto>.Success(subscriptionDto);
        }

        public async Task<Result> CreateSubscriptionAsync(CreateSubscriptionDto subscriptionDto)
        {
            // Verificar si el email ya existe
            var existingSubscription = await _subscriptionRepository.GetSubscriptionByEmailAsync(subscriptionDto.Email);
            if (existingSubscription != null)
            {
                if (existingSubscription.IsActive)
                {
                    return Result.Failure("Este email ya está suscrito a nuestro boletín.");
                }
                else
                {
                    // Reactivar suscripción existente
                    existingSubscription.IsActive = true;
                    existingSubscription.Name = subscriptionDto.Name;
                    existingSubscription.Frequency = subscriptionDto.Frequency;
                    existingSubscription.SubscriptionDate = DateTime.UtcNow;
                    existingSubscription.ConfirmationToken = Guid.NewGuid().ToString();

                    await _subscriptionRepository.UpdateSubscriptionAsync(existingSubscription);
                    await SendConfirmationEmailAsync(existingSubscription);
                    return Result.Success();
                }
            }

            // Crear nueva suscripción
            var newSubscription = new NewsletterSubscription
            {
                Email = subscriptionDto.Email,
                Name = subscriptionDto.Name,
                Frequency = subscriptionDto.Frequency,
                IsActive = false, // Inactiva hasta confirmar email
                SubscriptionDate = DateTime.UtcNow,
                ConfirmationToken = Guid.NewGuid().ToString()
            };

            await _subscriptionRepository.CreateSubscriptionAsync(newSubscription);
            await SendConfirmationEmailAsync(newSubscription);
            return Result.Success();
        }

        public async Task<Result> ConfirmSubscriptionAsync(string token)
        {
            var subscription = await _subscriptionRepository.GetSubscriptionByTokenAsync(token);
            if (subscription == null)
            {
                return Result.Failure("Token de confirmación inválido.");
            }

            subscription.IsActive = true;
            subscription.ConfirmationToken = string.Empty; // Limpiar token usado
            await _subscriptionRepository.UpdateSubscriptionAsync(subscription);

            return Result.Success();
        }

        public async Task<Result> UnsubscribeAsync(string email)
        {
            var subscription = await _subscriptionRepository.GetSubscriptionByEmailAsync(email);
            if (subscription == null)
            {
                return Result.Failure("Suscripción no encontrada.");
            }

            subscription.IsActive = false;
            await _subscriptionRepository.UpdateSubscriptionAsync(subscription);
            return Result.Success();
        }

        public async Task<Result> ToggleSubscriptionStatusAsync(int id)
        {
            var subscription = await _subscriptionRepository.GetSubscriptionByIdAsync(id);
            if (subscription == null)
            {
                return Result.Failure("Suscripción no encontrada.");
            }

            subscription.IsActive = !subscription.IsActive;
            await _subscriptionRepository.UpdateSubscriptionAsync(subscription);
            return Result.Success();
        }

        public async Task<Result<int>> GetActiveSubscriptionsCountAsync()
        {
            var count = await _subscriptionRepository.GetActiveSubscriptionsCountAsync();
            return Result<int>.Success(count);
        }

        public async Task<Result<IEnumerable<SubscriptionToListDto>>> GetSubscriptionsByFrequencyAsync(int frequency)
        {
            var subscriptionFrequency = (SubscriptionFrequency)frequency;
            var subscriptions = await _subscriptionRepository.GetSubscriptionsByFrequencyAsync(subscriptionFrequency);

            var subscriptionDtos = subscriptions.Select(s => new SubscriptionToListDto
            {
                Id = s.Id,
                Email = s.Email,
                Name = s.Name,
                IsActive = s.IsActive,
                SubscriptionDate = s.SubscriptionDate,
                Frequency = s.Frequency.ToString()
            });

            return Result<IEnumerable<SubscriptionToListDto>>.Success(subscriptionDtos);
        }

        private async Task SendConfirmationEmailAsync(NewsletterSubscription subscription)
        {
            var subject = "Confirmar suscripción al boletín";
            var confirmationLink = $"https://tu-dominio.com/confirm-subscription?token={subscription.ConfirmationToken}";
            var body = $@"
                <p>Hola {subscription.Name},</p>
                <p>Gracias por suscribirte a nuestro boletín.</p>
                <p>Para confirmar tu suscripción, haz clic en el siguiente enlace:</p>
                <p><a href='{confirmationLink}'>Confirmar suscripción</a></p>
                <p>Si no solicitaste esta suscripción, puedes ignorar este mensaje.</p>
                <p>Gracias,<br>El equipo de la fundación</p>";

            await _emailService.SendEmailAsync(subscription.Email, subject, body);
        }
    }
}

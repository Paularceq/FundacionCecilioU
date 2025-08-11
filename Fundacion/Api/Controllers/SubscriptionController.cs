using Api.Abstractions.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.PublicContent;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        // ===============================================
        // ENDPOINTS PÚBLICOS (MANTENER WRAPPERS)
        // ===============================================

        [HttpPost("Subscribe")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionDto subscriptionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _subscriptionService.CreateSubscriptionAsync(subscriptionDto);

            if (result.IsFailure)
            {
                return BadRequest(new
                {
                    success = false,
                    message = result.Errors.FirstOrDefault() ?? "Error al crear la suscripción.",
                    errors = result.Errors
                });
            }

            return Ok(new
            {
                success = true,
                message = "¡Suscripción creada exitosamente! Revisa tu email para confirmar tu suscripción."
            });
        }

        [HttpGet("Confirm")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmSubscription([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Token de confirmación requerido."
                });
            }

            var result = await _subscriptionService.ConfirmSubscriptionAsync(token);

            if (result.IsFailure)
            {
                return BadRequest(new
                {
                    success = false,
                    message = result.Errors.FirstOrDefault() ?? "Token inválido o expirado.",
                    errors = result.Errors
                });
            }

            return Ok(new
            {
                success = true,
                message = "¡Suscripción confirmada exitosamente! Ya recibirás nuestros boletines."
            });
        }

        [HttpPost("Unsubscribe")]
        [AllowAnonymous]
        public async Task<IActionResult> Unsubscribe([FromBody] UnsubscribeRequestDto request)
        {
            if (string.IsNullOrEmpty(request?.Email))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Email requerido para darse de baja."
                });
            }

            var result = await _subscriptionService.UnsubscribeAsync(request.Email);

            if (result.IsFailure)
            {
                return BadRequest(new
                {
                    success = false,
                    message = result.Errors.FirstOrDefault() ?? "Error al procesar la baja.",
                    errors = result.Errors
                });
            }

            return Ok(new
            {
                success = true,
                message = "Te has dado de baja exitosamente. Lamentamos verte partir."
            });
        }

        // ===============================================
        // ENDPOINTS ADMINISTRATIVOS (✅ CORREGIDOS - DATOS DIRECTOS)
        // ===============================================

        [HttpGet("All")]
        [Authorize]
        public async Task<IActionResult> GetAllSubscriptions()
        {
            var result = await _subscriptionService.GetAllSubscriptionsAsync();
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }

            // ✅ CAMBIO: Devolver datos directamente
            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetSubscriptionById(int id)
        {
            var result = await _subscriptionService.GetSubscriptionByIdAsync(id);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }

            // ✅ CAMBIO: Devolver datos directamente
            return Ok(result.Value);
        }

        [HttpPost("ToggleStatus/{id}")]
        [Authorize]
        public async Task<IActionResult> ToggleSubscriptionStatus(int id)
        {
            var result = await _subscriptionService.ToggleSubscriptionStatusAsync(id);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }

            return NoContent(); // ✅ CAMBIO: NoContent() como otros endpoints similares
        }

        [HttpGet("Count")]
        [Authorize]
        public async Task<IActionResult> GetActiveSubscriptionsCount()
        {
            var result = await _subscriptionService.GetActiveSubscriptionsCountAsync();
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }

            // ✅ CAMBIO: Devolver número directamente
            return Ok(result.Value);
        }

        [HttpGet("ByFrequency/{frequency}")]
        [Authorize]
        public async Task<IActionResult> GetSubscriptionsByFrequency(int frequency)
        {
            if (frequency < 1 || frequency > 3)
            {
                return BadRequest("Frecuencia inválida. Use: 1=Diario, 2=Semanal, 3=Mensual");
            }

            var result = await _subscriptionService.GetSubscriptionsByFrequencyAsync(frequency);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }

            // ✅ CAMBIO: Devolver datos directamente
            return Ok(result.Value);
        }

        // OPCIONAL: Mantener Statistics con wrapper para dashboard
        [HttpGet("Statistics")]
        [Authorize]
        public async Task<IActionResult> GetSubscriptionStatistics()
        {
            var allSubscriptions = await _subscriptionService.GetAllSubscriptionsAsync();

            if (allSubscriptions.IsFailure)
            {
                return BadRequest(allSubscriptions.Errors);
            }

            var subscriptions = allSubscriptions.Value;
            var totalSubscriptions = subscriptions.Count();
            var activeSubscriptions = subscriptions.Count(s => s.IsActive);
            var inactiveSubscriptions = totalSubscriptions - activeSubscriptions;

            var dailySubscriptions = subscriptions.Count(s => s.IsActive && s.Frequency == "Daily");
            var weeklySubscriptions = subscriptions.Count(s => s.IsActive && s.Frequency == "Weekly");
            var monthlySubscriptions = subscriptions.Count(s => s.IsActive && s.Frequency == "Monthly");

            // ✅ MANTENER wrapper para estadísticas (más útil así)
            return Ok(new
            {
                success = true,
                statistics = new
                {
                    total = totalSubscriptions,
                    active = activeSubscriptions,
                    inactive = inactiveSubscriptions,
                    byFrequency = new
                    {
                        daily = dailySubscriptions,
                        weekly = weeklySubscriptions,
                        monthly = monthlySubscriptions
                    },
                    growthThisMonth = subscriptions.Count(s => s.SubscriptionDate >= DateTime.UtcNow.AddDays(-30))
                }
            });
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Extensions;
using Web.Models.Newsletter;
using Web.Services;

namespace Web.Controllers
{
    public class SubscriptionController : Controller
    {
        private readonly SubscriptionService _subscriptionService;

        public SubscriptionController(SubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        // ===============================================
        // ENDPOINTS PÚBLICOS (Sin autenticación)
        // ===============================================

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Subscribe()
        {
            var model = new SubscribeViewModel();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SubscribeAsync(SubscribeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _subscriptionService.CreateSubscriptionAsync(model);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                return View(model);
            }

            this.SetSuccessMessage("¡Suscripción exitosa! Revisa tu email para confirmar tu suscripción.");
            return View("SubscribeSuccess");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                this.SetErrorMessage("Token de confirmación inválido.");
                return View("ConfirmError");
            }

            var result = await _subscriptionService.ConfirmSubscriptionAsync(token);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                return View("ConfirmError");
            }

            return View("ConfirmSuccess");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Unsubscribe()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> UnsubscribeAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                this.SetErrorMessage("Email requerido.");
                return View();
            }

            var result = await _subscriptionService.UnsubscribeAsync(email);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                return View();
            }

            return View("UnsubscribeSuccess");
        }

        // ===============================================
        // ENDPOINTS ADMINISTRATIVOS (Con autenticación)
        // ===============================================

        [HttpGet]
        [Authorize(Roles = "AdminUsuarios")]
        public async Task<IActionResult> IndexAsync()
        {
            var subscriptions = await _subscriptionService.GetAllSubscriptionsAsync();
            return View(subscriptions.Value);
        }

        [HttpGet]
        [Authorize(Roles = "AdminUsuarios")]
        public async Task<IActionResult> DetailsAsync(int id)
        {
            var subscription = await _subscriptionService.GetSubscriptionByIdAsync(id);
            if (subscription.IsFailure)
            {
                this.SetErrorMessage(subscription.Errors);
                return RedirectToAction("Index");
            }

            return View(subscription.Value);
        }

        [HttpGet]
        [Authorize(Roles = "AdminUsuarios")]
        public async Task<IActionResult> ChangeSubscriptionStatusAsync(int id)
        {
            var subscription = await _subscriptionService.GetSubscriptionByIdAsync(id);
            if (subscription.IsFailure)
            {
                this.SetErrorMessage(subscription.Errors);
                return RedirectToAction("Index");
            }

            var result = await _subscriptionService.ToggleSubscriptionStatusAsync(id);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                return RedirectToAction("Index");
            }

            this.SetSuccessMessage($"La suscripción de {subscription.Value.Email} ha sido {(subscription.Value.IsActive ? "desactivada" : "activada")} correctamente.");
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "AdminUsuarios")]
        public async Task<IActionResult> StatisticsAsync()
        {
            var stats = await _subscriptionService.GetSubscriptionStatisticsAsync();
            if (stats.IsFailure)
            {
                this.SetErrorMessage(stats.Errors);
                return RedirectToAction("Index");
            }

            return View(stats.Value);
        }
    }
}
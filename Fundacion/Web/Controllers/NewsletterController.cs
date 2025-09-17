using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.Extensions;
using Web.Models.Newsletter;
using Web.Services;

namespace Web.Controllers
{
    [Authorize(Roles = "AdminUsuarios")] // Ajustar rol según tu sistema
    public class NewsletterController : Controller
    {
        private readonly NewsletterService _newsletterService;
        private readonly SubscriptionService _subscriptionService;

        public NewsletterController(NewsletterService newsletterService, SubscriptionService subscriptionService)
        {
            _newsletterService = newsletterService;
            _subscriptionService = subscriptionService;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            var newsletters = await _newsletterService.GetAllNewslettersAsync();
            return View(newsletters.Value);
        }

        [HttpGet]
        public IActionResult CreateNewsletter()
        {
            var model = new CreateNewsletterViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewsletterAsync(CreateNewsletterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _newsletterService.CreateNewsletterAsync(model);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                return View(model);
            }

            if (model.SendNow)
            {
                this.SetSuccessMessage("Newsletter creado y enviado correctamente.");
            }
            else
            {
                this.SetSuccessMessage("Newsletter creado correctamente.");
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> SendNewsletterAsync(int id)
        {
            var newsletter = await _newsletterService.GetNewsletterByIdAsync(id);
            if (newsletter.IsFailure)
            {
                this.SetErrorMessage(newsletter.Errors);
                return RedirectToAction("Index");
            }

            if (!newsletter.Value.CanBeSent)
            {
                this.SetErrorMessage("Este newsletter no puede ser enviado en su estado actual.");
                return RedirectToAction("Index");
            }

            var result = await _newsletterService.SendNewsletterAsync(id);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                return RedirectToAction("Index");
            }

            this.SetSuccessMessage($"El newsletter '{newsletter.Value.Subject}' ha sido enviado correctamente.");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteNewsletterAsync(int id)
        {
            var newsletter = await _newsletterService.GetNewsletterByIdAsync(id);
            if (newsletter.IsFailure)
            {
                this.SetErrorMessage(newsletter.Errors);
                return RedirectToAction("Index");
            }

            if (!newsletter.Value.CanBeDeleted)
            {
                this.SetErrorMessage("No se puede eliminar un newsletter que ya ha sido enviado.");
                return RedirectToAction("Index");
            }

            var result = await _newsletterService.DeleteNewsletterAsync(id);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                return RedirectToAction("Index");
            }

            this.SetSuccessMessage($"El newsletter '{newsletter.Value.Subject}' ha sido eliminado correctamente.");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> DetailsAsync(int id)
        {
            var newsletter = await _newsletterService.GetNewsletterByIdAsync(id);
            if (newsletter.IsFailure)
            {
                this.SetErrorMessage(newsletter.Errors);
                return RedirectToAction("Index");
            }

            return View(newsletter.Value);
        }

        [HttpPost]
        public async Task<IActionResult> PreviewNewsletterAsync(CreateNewsletterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Datos inválidos");
            }

            var result = await _newsletterService.GenerateNewsletterPreviewAsync(model);
            if (result.IsFailure)
            {
                return BadRequest("Error al generar vista previa");
            }

            return Content(result.Value, "text/html");
        }

        [HttpGet]
        public async Task<IActionResult> StatisticsAsync()
        {
            var subscriptionsCount = await _subscriptionService.GetActiveSubscriptionsCountAsync();
            var subscriptionsStats = await _subscriptionService.GetSubscriptionStatisticsAsync();
            var newsletters = await _newsletterService.GetAllNewslettersAsync();

            var model = new NewsletterStatsViewModel();

            if (subscriptionsCount.IsSuccess)
            {
                model.ActiveSubscriptions = subscriptionsCount.Value;
            }

            if (subscriptionsStats.IsSuccess)
            {
                model.TotalSubscriptions = subscriptionsStats.Value.TotalSubscriptions;
                model.InactiveSubscriptions = subscriptionsStats.Value.InactiveSubscriptions;
                model.DailySubscriptions = subscriptionsStats.Value.DailySubscriptions;
                model.WeeklySubscriptions = subscriptionsStats.Value.WeeklySubscriptions;
                model.MonthlySubscriptions = subscriptionsStats.Value.MonthlySubscriptions;
                model.GrowthThisMonth = subscriptionsStats.Value.GrowthThisMonth;
            }

            if (newsletters.IsSuccess)
            {
                model.TotalNewsletters = newsletters.Value.Count();
                model.NewslettersSent = newsletters.Value.Count(n => n.Status == Shared.Enums.NewsletterStatus.Sent);
            }

            return View(model);
        }
    }
}
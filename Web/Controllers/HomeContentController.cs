using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.Extensions;
using Web.Models.Newsletter;
using Web.Services;

namespace Web.Controllers
{
    [Authorize(Roles = "AdminUsuarios")] // Ajustar rol según tu sistema
    public class HomeContentController : Controller
    {
        private readonly HomeContentService _homeContentService;

        public HomeContentController(HomeContentService homeContentService)
        {
            _homeContentService = homeContentService;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            var homeContents = await _homeContentService.GetAllHomeContentAsync();
            return View(homeContents.Value);
        }

        [HttpGet]
        public IActionResult AddHomeContent()
        {
            var model = new CreateHomeContentViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddHomeContentAsync(CreateHomeContentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _homeContentService.CreateHomeContentAsync(model);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                return View(model);
            }

            this.SetSuccessMessage("Contenido creado correctamente.");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateHomeContentAsync(int id)
        {
            var homeContent = await _homeContentService.GetHomeContentByIdAsync(id);
            if (homeContent.IsFailure)
            {
                this.SetErrorMessage(homeContent.Errors);
                return RedirectToAction("Index");
            }

            var model = new UpdateHomeContentViewModel
            {
                Id = homeContent.Value.Id,
                Title = homeContent.Value.Title,
                Description = homeContent.Value.Description,
                ImageUrl = homeContent.Value.ImageUrl,
                IsActive = homeContent.Value.IsActive,
                StartDate = homeContent.Value.StartDate,
                EndDate = homeContent.Value.EndDate
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateHomeContentAsync(UpdateHomeContentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _homeContentService.UpdateHomeContentAsync(model);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                return View(model);
            }

            this.SetSuccessMessage("Contenido actualizado correctamente.");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ChangeHomeContentStatusAsync(int id)
        {
            var homeContent = await _homeContentService.GetHomeContentByIdAsync(id);
            if (homeContent.IsFailure)
            {
                this.SetErrorMessage(homeContent.Errors);
                return RedirectToAction("Index");
            }

            var result = await _homeContentService.ToggleHomeContentStatusAsync(id);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                return RedirectToAction("Index");
            }

            this.SetSuccessMessage($"El contenido '{homeContent.Value.Title}' ha sido {(homeContent.Value.IsActive ? "desactivado" : "activado")} correctamente.");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteHomeContentAsync(int id)
        {
            var homeContent = await _homeContentService.GetHomeContentByIdAsync(id);
            if (homeContent.IsFailure)
            {
                this.SetErrorMessage(homeContent.Errors);
                return RedirectToAction("Index");
            }

            var result = await _homeContentService.DeleteHomeContentAsync(id);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                return RedirectToAction("Index");
            }

            this.SetSuccessMessage($"El contenido '{homeContent.Value.Title}' ha sido eliminado correctamente.");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> DetailsAsync(int id)
        {
            var homeContent = await _homeContentService.GetHomeContentByIdAsync(id);
            if (homeContent.IsFailure)
            {
                this.SetErrorMessage(homeContent.Errors);
                return RedirectToAction("Index");
            }

            return View(homeContent.Value);
        }
    }
}
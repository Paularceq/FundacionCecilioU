using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Constants;
using Web.Extensions;
using Web.Models.Volunteer;
using Web.Services;

namespace Web.Controllers
{
    [Authorize(Roles = Roles.Voluntario)]
    public class VolunteerRequestController : Controller
    {
        private readonly VolunteerRequestService _volunteerRequestService;
        public VolunteerRequestController(VolunteerRequestService volunteerRequestService)
        {
            _volunteerRequestService = volunteerRequestService;
        }
        public async Task<IActionResult> IndexAsync()
        {
            int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var requests = await _volunteerRequestService.GetAllByVolunteerIDAsync(userId);
            return View(requests);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateRequestViewModel requestViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(requestViewModel);
            }
            int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var result = await _volunteerRequestService.CreateAsync(requestViewModel, userId);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                return View(requestViewModel);
            }
            return RedirectToAction("Index");
        }
    }
}

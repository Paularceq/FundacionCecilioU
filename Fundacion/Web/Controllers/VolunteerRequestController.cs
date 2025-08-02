using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Constants;
using Shared.Enums;
using Web.Extensions;
using Web.Models.Volunteer;
using Web.Services;

namespace Web.Controllers
{
    [Authorize(Roles = Roles.Voluntario)]
    public class VolunteerRequestController : Controller
    {
        private readonly VolunteerRequestService _volunteerRequestService;
        private readonly VolunteerHoursService _volunteerHoursService;

        public VolunteerRequestController(
            VolunteerRequestService volunteerRequestService,
            VolunteerHoursService volunteerHoursService)
        {
            _volunteerRequestService = volunteerRequestService;
            _volunteerHoursService = volunteerHoursService;
        }

        public async Task<IActionResult> IndexAsync()
        {
            int userId = GetCurrentUserId();
            var requests = await _volunteerRequestService.GetAllByVolunteerIDAsync(userId);
            return View(requests);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateRequestViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            int userId = GetCurrentUserId();
            var result = await _volunteerRequestService.CreateAsync(model, userId);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                return View(model);
            }

            return RedirectToAction("Index");
        }

        // ===== GESTIÓN DE HORAS =====
        [HttpGet]
        public async Task<IActionResult> ManageHours(int requestId)
        {
            var request = await _volunteerRequestService.GetRequestByIdAsync(requestId);
            if (request.IsFailure || request.Value.State != VolunteerState.Approved)
            {
                this.SetErrorMessage("Solo se pueden gestionar horas para solicitudes aprobadas");
                return RedirectToAction("Index");
            }

            var hours = await _volunteerHoursService.GetHoursByRequestIdAsync(requestId);
            if (hours.IsFailure)
            {
                this.SetErrorMessage(hours.Errors);
                return RedirectToAction("Index");
            }

            var requestDto = request.Value;

            var viewModel = new ManageHoursViewModel
            {
                RequestId = requestId,
                VolunteerName = requestDto.VolunteerName ?? string.Empty,
                TotalHoursRequested = requestDto.Hours,
                HoursList = hours.Value ?? new(),
                CanAddMore = requestDto.RemainingHours > 0
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> AddHours(int requestId)
        {
            var request = await _volunteerRequestService.GetRequestByIdAsync(requestId);
            if (request.IsFailure || request.Value.State != VolunteerState.Approved)
            {
                this.SetErrorMessage("Solo se pueden registrar horas para solicitudes aprobadas");
                return RedirectToAction("ManageHours", new { requestId });
            }

            var requestDto = request.Value;

            if (requestDto.RemainingHours <= 0)
            {
                this.SetErrorMessage("Ya no tienes horas disponibles en esta solicitud");
                return RedirectToAction("ManageHours", new { requestId });
            }

            if (await _volunteerHoursService.HasHoursForDateAsync(requestId, DateTime.Today))
            {
                this.SetErrorMessage("Ya existe un registro de horas para hoy");
                return RedirectToAction("ManageHours", new { requestId });
            }

            var model = new AddHoursViewModel
            {
                RequestId = requestId,
                Date = DateTime.Today,
                StartTime = TimeSpan.FromHours(8),
                EndTime = TimeSpan.FromHours(17)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddHours(AddHoursViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _volunteerHoursService.CreateVolunteerHoursAsync(model);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                return View(model);
            }

            this.SetSuccessMessage("Horas registradas correctamente. Pendiente de aprobación.");
            return RedirectToAction("ManageHours", new { requestId = model.RequestId });
        }

        [HttpGet]
        public async Task<IActionResult> EditHours(int hoursId)
        {
            var result = await _volunteerHoursService.GetHoursByIdAsync(hoursId);
            if (result.IsFailure)
            {
                this.SetErrorMessage("Registro de horas no encontrado");
                return RedirectToAction("Index");
            }

            var hours = result.Value;
            if (hours.State == VolunteerState.Approved)
            {
                this.SetErrorMessage("No se pueden editar horas ya aprobadas");
                return RedirectToAction("ManageHours", new { requestId = hours.VolunteerRequestId });
            }

            var model = new AddHoursViewModel
            {
                Id = hours.Id,
                RequestId = hours.VolunteerRequestId,
                Date = hours.Date,
                StartTime = hours.StartTime,
                EndTime = hours.EndTime,
                ActivitiesDescription = hours.ActivitiesDescription,
                Notes = hours.Notes
            };

            return View("AddHours", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditHours(AddHoursViewModel model)
        {
            if (!ModelState.IsValid) return View("AddHours", model);

            var result = await _volunteerHoursService.UpdateVolunteerHoursAsync(model.Id!.Value, model);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                return View("AddHours", model);
            }

            this.SetSuccessMessage("Horas actualizadas correctamente");
            return RedirectToAction("ManageHours", new { requestId = model.RequestId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteHours(int hoursId, int requestId)
        {
            var result = await _volunteerHoursService.DeleteVolunteerHoursAsync(hoursId);
            if (result.IsFailure)
                this.SetErrorMessage(result.Errors);
            else
                this.SetSuccessMessage("Registro de horas eliminado correctamente");

            return RedirectToAction("ManageHours", new { requestId });
        }

        [HttpPost]
        public async Task<JsonResult> ValidateHours([FromBody] AddHoursViewModel model)
        {
            var result = await _volunteerHoursService.ValidateHoursAsync(model);
            return Json(new
            {
                isValid = result.IsSuccess,
                errors = result.Errors
            });
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        }
    }
}

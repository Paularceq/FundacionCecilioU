using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using Web.Extensions;
using Web.Models.Volunteer;
using Web.Services;

namespace Web.Controllers
{
    public class VolunteerController : Controller
    {
        private readonly VolunteerRequestService _volunteerRequestService;
        private readonly VolunteerHoursService _volunteerHoursService;

        public VolunteerController(
            VolunteerRequestService volunteerRequestService,
            VolunteerHoursService volunteerHoursService)
        {
            _volunteerRequestService = volunteerRequestService;
            _volunteerHoursService = volunteerHoursService;
        }

        // ===== GESTIÓN DE SOLICITUDES =====
        public async Task<IActionResult> Index()
        {
            var volunteerId = GetCurrentUserId(); // Método helper para obtener ID del usuario actual
            var requests = await _volunteerRequestService.GetAllByVolunteerIDAsync(volunteerId);
            return View(requests);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateRequestViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var volunteerId = GetCurrentUserId();
            var result = await _volunteerRequestService.CreateAsync(model, volunteerId);

            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                return View(model);
            }

            this.SetSuccessMessage("Solicitud de voluntariado creada correctamente");
            return RedirectToAction(nameof(Index));
        }


        // ===== GESTIÓN DE HORAS =====
        [HttpGet]
        public async Task<IActionResult> ManageHours(int requestId)
        {
            // Verificar que la solicitud está aprobada
            var requestResult = await _volunteerRequestService.GetRequestByIdAsync(requestId);
            if (requestResult.IsFailure || requestResult.Value.State != VolunteerState.Approved)
            {
                this.SetErrorMessage("Solo se pueden gestionar horas para solicitudes aprobadas");
                return RedirectToAction(nameof(Index));
            }

            var hoursResult = await _volunteerHoursService.GetHoursByRequestIdAsync(requestId);

            var viewModel = new ManageHoursViewModel
            {
                RequestId = requestId,
                VolunteerName = requestResult.Value.VolunteerName,
                HoursList = hoursResult.Value ?? new(),
                CanAddMore = true // Se puede validar si ya completó las horas
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> AddHours(int requestId)
        {
            // Verificar que puede registrar horas
            var canRegister = await _volunteerHoursService.CanRegisterHoursAsync(requestId, DateTime.Today);
            if (!canRegister)
            {
                this.SetErrorMessage("No se pueden registrar más horas para hoy o la solicitud no está aprobada");
                return RedirectToAction(nameof(ManageHours), new { requestId });
            }

            var model = new AddHoursViewModel
            {
                RequestId = requestId,
                Date = DateTime.Today,
                StartTime = TimeSpan.FromHours(8), // 8:00 AM por defecto
                EndTime = TimeSpan.FromHours(17)   // 5:00 PM por defecto
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddHours(AddHoursViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _volunteerHoursService.CreateVolunteerHoursAsync(model);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                return View(model);
            }

            this.SetSuccessMessage("Horas registradas correctamente. Pendiente de aprobación.");
            return RedirectToAction(nameof(ManageHours), new { requestId = model.RequestId });
        }

        [HttpGet]
        public async Task<IActionResult> EditHours(int hoursId)
        {
            var hoursResult = await _volunteerHoursService.GetHoursByIdAsync(hoursId);
            if (hoursResult.IsFailure)
            {
                this.SetErrorMessage("Registro de horas no encontrado");
                return RedirectToAction(nameof(Index));
            }

            var hours = hoursResult.Value;
            if (hours.State == VolunteerState.Approved)
            {
                this.SetErrorMessage("No se pueden editar horas ya aprobadas");
                return RedirectToAction(nameof(ManageHours), new { requestId = hours.VolunteerRequestId });
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
            if (!ModelState.IsValid)
            {
                return View("AddHours", model);
            }

            var result = await _volunteerHoursService.UpdateVolunteerHoursAsync(model.Id!.Value, model);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                return View("AddHours", model);
            }

            this.SetSuccessMessage("Horas actualizadas correctamente");
            return RedirectToAction(nameof(ManageHours), new { requestId = model.RequestId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteHours(int hoursId, int requestId)
        {
            var result = await _volunteerHoursService.DeleteVolunteerHoursAsync(hoursId);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
            }
            else
            {
                this.SetSuccessMessage("Registro de horas eliminado correctamente");
            }

            return RedirectToAction(nameof(ManageHours), new { requestId });
        }

       
        // ===== API ENDPOINTS PARA AJAX =====

       

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

        // ===== MÉTODO HELPER =====
        private int GetCurrentUserId()
        {
            // Implementar según tu sistema de autenticación
            // Por ejemplo, obtener del JWT token o session
            return int.Parse(User.FindFirst("UserId")?.Value ?? "0");
        }
    }
}
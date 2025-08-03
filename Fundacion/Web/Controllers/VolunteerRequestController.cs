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

        // ===== GESTIÓN DE SOLICITUDES =====
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

            this.SetSuccessMessage("Solicitud creada correctamente. Pendiente de aprobación.");
            return RedirectToAction("Index");
        }

        // ===== GESTIÓN DE HORAS =====
        [HttpGet]
        public async Task<IActionResult> ManageHours(int requestId)
        {
            // Verificar que la solicitud existe y pertenece al usuario actual
            var request = await _volunteerRequestService.GetRequestByIdAsync(requestId);
            if (request.IsFailure)
            {
                this.SetErrorMessage("Solicitud no encontrada");
                return RedirectToAction("Index");
            }

            var requestDto = request.Value;

            // Verificar que la solicitud pertenece al usuario actual
            if (requestDto.VolunteerId != GetCurrentUserId())
            {
                this.SetErrorMessage("No tienes permisos para gestionar esta solicitud");
                return RedirectToAction("Index");
            }

            // Verificar que la solicitud está aprobada
            if (requestDto.State != VolunteerState.Approved)
            {
                this.SetErrorMessage("Solo se pueden gestionar horas para solicitudes aprobadas");
                return RedirectToAction("Index");
            }

            // Obtener las horas registradas
            var hours = await _volunteerHoursService.GetHoursByRequestIdAsync(requestId);
            if (hours.IsFailure)
            {
                this.SetErrorMessage(hours.Errors);
                return RedirectToAction("Index");
            }

            var viewModel = new ManageHoursViewModel
            {
                RequestId = requestId,
                VolunteerName = requestDto.VolunteerName ?? string.Empty,
                Institution = requestDto.Institution,
                TotalHoursRequested = requestDto.Hours,
                HoursList = hours.Value ?? new(),
                CanAddMore = requestDto.RemainingHours > 0 && !await _volunteerHoursService.HasHoursForDateAsync(requestId, DateTime.Today)
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> AddHours(int requestId)
        {
            // Verificar que la solicitud existe y pertenece al usuario actual
            var request = await _volunteerRequestService.GetRequestByIdAsync(requestId);
            if (request.IsFailure)
            {
                this.SetErrorMessage("Solicitud no encontrada");
                return RedirectToAction("Index");
            }

            var requestDto = request.Value;

            // Verificar que la solicitud pertenece al usuario actual
            if (requestDto.VolunteerId != GetCurrentUserId())
            {
                this.SetErrorMessage("No tienes permisos para registrar horas en esta solicitud");
                return RedirectToAction("Index");
            }

            // Verificar que la solicitud está aprobada
            if (requestDto.State != VolunteerState.Approved)
            {
                this.SetErrorMessage("Solo se pueden registrar horas para solicitudes aprobadas");
                return RedirectToAction("ManageHours", new { requestId });
            }

            // Verificar que quedan horas disponibles
            if (requestDto.RemainingHours <= 0)
            {
                this.SetErrorMessage("Ya no tienes horas disponibles en esta solicitud");
                return RedirectToAction("ManageHours", new { requestId });
            }

            // Verificar que no hay registro para hoy
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

            ViewBag.RequestInfo = new
            {
                Institution = requestDto.Institution,
                RemainingHours = requestDto.RemainingHours
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddHours(AddHoursViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Recargar información de la solicitud para la vista
                var requestInfo = await _volunteerRequestService.GetRequestByIdAsync(model.RequestId);
                if (requestInfo.IsSuccess)
                {
                    ViewBag.RequestInfo = new
                    {
                        Institution = requestInfo.Value.Institution,
                        RemainingHours = requestInfo.Value.RemainingHours
                    };
                }
                return View(model);
            }

            // Validaciones adicionales antes de enviar
            var request = await _volunteerRequestService.GetRequestByIdAsync(model.RequestId);
            if (request.IsFailure || request.Value.VolunteerId != GetCurrentUserId())
            {
                this.SetErrorMessage("No tienes permisos para registrar horas en esta solicitud");
                return RedirectToAction("Index");
            }

            var result = await _volunteerHoursService.CreateVolunteerHoursAsync(model);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);

                // Recargar información de la solicitud para la vista
                ViewBag.RequestInfo = new
                {
                    Institution = request.Value.Institution,
                    RemainingHours = request.Value.RemainingHours
                };

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

            // Verificar que la solicitud pertenece al usuario actual
            var request = await _volunteerRequestService.GetRequestByIdAsync(hours.VolunteerRequestId);
            if (request.IsFailure || request.Value.VolunteerId != GetCurrentUserId())
            {
                this.SetErrorMessage("No tienes permisos para editar este registro de horas");
                return RedirectToAction("Index");
            }

            // Verificar que las horas no están aprobadas
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

            ViewBag.RequestInfo = new
            {
                Institution = request.Value.Institution,
                RemainingHours = request.Value.RemainingHours,
                IsEditing = true
            };

            return View("AddHours", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditHours(AddHoursViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.RequestInfo = new { IsEditing = true };
                return View("AddHours", model);
            }

            // Verificar permisos
            var hoursResult = await _volunteerHoursService.GetHoursByIdAsync(model.Id!.Value);
            if (hoursResult.IsFailure)
            {
                this.SetErrorMessage("Registro de horas no encontrado");
                return RedirectToAction("Index");
            }

            var request = await _volunteerRequestService.GetRequestByIdAsync(model.RequestId);
            if (request.IsFailure || request.Value.VolunteerId != GetCurrentUserId())
            {
                this.SetErrorMessage("No tienes permisos para editar este registro de horas");
                return RedirectToAction("Index");
            }

            var result = await _volunteerHoursService.UpdateVolunteerHoursAsync(model.Id!.Value, model);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                ViewBag.RequestInfo = new { IsEditing = true };
                return View("AddHours", model);
            }

            this.SetSuccessMessage("Horas actualizadas correctamente");
            return RedirectToAction("ManageHours", new { requestId = model.RequestId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteHours(int hoursId, int requestId)
        {
            // Verificar permisos
            var hoursResult = await _volunteerHoursService.GetHoursByIdAsync(hoursId);
            if (hoursResult.IsFailure)
            {
                this.SetErrorMessage("Registro de horas no encontrado");
                return RedirectToAction("ManageHours", new { requestId });
            }

            var request = await _volunteerRequestService.GetRequestByIdAsync(requestId);
            if (request.IsFailure || request.Value.VolunteerId != GetCurrentUserId())
            {
                this.SetErrorMessage("No tienes permisos para eliminar este registro de horas");
                return RedirectToAction("Index");
            }

            var result = await _volunteerHoursService.DeleteVolunteerHoursAsync(hoursId);
            if (result.IsFailure)
                this.SetErrorMessage(result.Errors);
            else
                this.SetSuccessMessage("Registro de horas eliminado correctamente");

            return RedirectToAction("ManageHours", new { requestId });
        }

        // ===== ENDPOINTS AJAX =====
        [HttpPost]
        public async Task<JsonResult> ValidateHours([FromBody] AddHoursViewModel model)
        {
            try
            {
                var result = await _volunteerHoursService.ValidateHoursAsync(model);
                return Json(new
                {
                    isValid = result.IsSuccess,
                    errors = result.Errors
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isValid = false,
                    errors = new[] { "Error al validar las horas" }
                });
            }
        }

        [HttpGet]
        public async Task<JsonResult> CheckHoursForDate(int requestId, DateTime date)
        {
            try
            {
                var hasHours = await _volunteerHoursService.HasHoursForDateAsync(requestId, date);
                return Json(new { hasHours });
            }
            catch
            {
                return Json(new { hasHours = false });
            }
        }

        // ===== DASHBOARD/ESTADÍSTICAS (PLACEHOLDER) =====
        public async Task<IActionResult> Dashboard(int requestId)
        {
            var request = await _volunteerRequestService.GetRequestByIdAsync(requestId);
            if (request.IsFailure || request.Value.VolunteerId != GetCurrentUserId())
            {
                this.SetErrorMessage("No tienes permisos para ver este dashboard");
                return RedirectToAction("Index");
            }

            // Por ahora redirige a ManageHours, pero aquí podrías implementar un dashboard completo
            this.SetInfoMessage("Dashboard en desarrollo. Mostrando gestión de horas.");
            return RedirectToAction("ManageHours", new { requestId });
        }

        // ===== MÉTODOS AUXILIARES =====
        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        private string GetCurrentUserName()
        {
            return User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Voluntario";
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Constants;
using Shared.Dtos.Volunteer;
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
        public async Task<IActionResult> Index()
        {
            int userId = GetCurrentUserId();
            var requests = await _volunteerRequestService.GetAllByVolunteerIDAsync(userId);

            // MEJORADO: Usar VolunteerRequestListViewModel
            var canCreateNew = await _volunteerRequestService.CanCreateNewRequestAsync(userId);

            var viewModel = new VolunteerRequestListViewModel
            {
                Requests = requests,
                CanCreateNew = canCreateNew
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            int userId = GetCurrentUserId();

            // REQUERIMIENTO: No puede haber más de una solicitud activa por voluntario
            // PERO puede crear nueva si ya cumplió las horas de la anterior
            var canCreate = await _volunteerRequestService.CanCreateNewRequestAsync(userId);
            if (!canCreate)
            {
                var activeRequest = await _volunteerRequestService.GetActiveRequestAsync(userId);
                var message = activeRequest.IsSuccess && activeRequest.Value.State == VolunteerState.Pending
                    ? "Ya tienes una solicitud pendiente de aprobación. Espera a que sea aprobada antes de crear una nueva."
                    : "Ya tienes una solicitud aprobada con horas pendientes de completar. Completa las horas restantes antes de crear una nueva solicitud.";

                this.SetErrorMessage(message);
                return RedirectToAction("Index");
            }

            return View(new CreateRequestViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRequestViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            int userId = GetCurrentUserId();

            // Verificar nuevamente antes de crear
            var canCreate = await _volunteerRequestService.CanCreateNewRequestAsync(userId);
            if (!canCreate)
            {
                var activeRequest = await _volunteerRequestService.GetActiveRequestAsync(userId);
                var message = activeRequest.IsSuccess && activeRequest.Value.State == VolunteerState.Pending
                    ? "Ya tienes una solicitud pendiente de aprobación."
                    : "Ya tienes una solicitud aprobada con horas pendientes de completar.";

                this.SetErrorMessage(message);
                return RedirectToAction("Index");
            }

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

            // MEJORADO: Usar método del servicio para construir ViewModel
            var viewModel = await _volunteerHoursService.BuildManageHoursViewModelAsync(requestId, requestDto);

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> AddHours(int requestId)
        {
            // Verificar permisos y validaciones
            var validationResult = await ValidateRequestAccessAsync(requestId, requireApproved: true);
            if (validationResult.redirectResult != null)
                return validationResult.redirectResult;

            var requestDto = validationResult.request;

            // REQUERIMIENTO: No puede registrar horas si ya cumplió las horas comprometidas
            if (requestDto.RemainingHours <= 0)
            {
                this.SetErrorMessage("Ya has cumplido con todas las horas comprometidas para esta solicitud");
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
                await LoadRequestInfoForView(model.RequestId);
                return View(model);
            }

            // Validaciones de permisos
            var validationResult = await ValidateRequestAccessAsync(model.RequestId, requireApproved: true);
            if (validationResult.redirectResult != null)
                return validationResult.redirectResult;

            var result = await _volunteerHoursService.CreateVolunteerHoursAsync(model);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                await LoadRequestInfoForView(model.RequestId);
                return View(model);
            }

            this.SetSuccessMessage("Horas registradas correctamente. Pendiente de aprobación.");
            return RedirectToAction("ManageHours", new { requestId = model.RequestId });
        }

        [HttpGet]
        public async Task<IActionResult> EditHours(int hoursId)
        {
            var hoursResult = await _volunteerHoursService.GetHoursByIdAsync(hoursId);
            if (hoursResult.IsFailure)
            {
                this.SetErrorMessage("Registro de horas no encontrado");
                return RedirectToAction("Index");
            }

            var hours = hoursResult.Value;

            // Verificar permisos
            var validationResult = await ValidateRequestAccessAsync(hours.VolunteerRequestId, requireApproved: true);
            if (validationResult.redirectResult != null)
                return validationResult.redirectResult;

            // REQUERIMIENTO: No se pueden editar horas ya aprobadas
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
                Institution = validationResult.request.Institution,
                RemainingHours = validationResult.request.RemainingHours,
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
            var validationResult = await ValidateRequestAccessAsync(model.RequestId, requireApproved: true);
            if (validationResult.redirectResult != null)
                return validationResult.redirectResult;

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
            var validationResult = await ValidateRequestAccessAsync(requestId, requireApproved: true);
            if (validationResult.redirectResult != null)
                return validationResult.redirectResult;

            var hoursResult = await _volunteerHoursService.GetHoursByIdAsync(hoursId);
            if (hoursResult.IsFailure)
            {
                this.SetErrorMessage("Registro de horas no encontrado");
                return RedirectToAction("ManageHours", new { requestId });
            }

            // REQUERIMIENTO: No se pueden eliminar horas ya aprobadas
            if (hoursResult.Value.State == VolunteerState.Approved)
            {
                this.SetErrorMessage("No se pueden eliminar horas ya aprobadas");
                return RedirectToAction("ManageHours", new { requestId });
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
            catch (Exception)
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

        // ===== MÉTODOS AUXILIARES =====
        private async Task<(IActionResult? redirectResult, VolunteerRequestDto request)> ValidateRequestAccessAsync(int requestId, bool requireApproved = false)
        {
            var requestResult = await _volunteerRequestService.GetRequestByIdAsync(requestId);
            if (requestResult.IsFailure)
            {
                this.SetErrorMessage("Solicitud no encontrada");
                return (RedirectToAction("Index"), null!);
            }

            var request = requestResult.Value;

            // Verificar que pertenece al usuario actual
            if (request.VolunteerId != GetCurrentUserId())
            {
                this.SetErrorMessage("No tienes permisos para acceder a esta solicitud");
                return (RedirectToAction("Index"), null!);
            }

            // Verificar que está aprobada si se requiere
            if (requireApproved && request.State != VolunteerState.Approved)
            {
                this.SetErrorMessage("Solo se pueden gestionar horas para solicitudes aprobadas");
                return (RedirectToAction("Index"), null!);
            }

            return (null, request);
        }

        private async Task LoadRequestInfoForView(int requestId)
        {
            var requestInfo = await _volunteerRequestService.GetRequestByIdAsync(requestId);
            if (requestInfo.IsSuccess)
            {
                ViewBag.RequestInfo = new
                {
                    Institution = requestInfo.Value.Institution,
                    RemainingHours = requestInfo.Value.RemainingHours
                };
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : 0;
        }

        private string GetCurrentUserName()
        {
            return User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Voluntario";
        }
    }
}
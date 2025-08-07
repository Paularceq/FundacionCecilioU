using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Constants;
using Shared.Dtos.Volunteer;
using Shared.Enums;
using Shared.Models;
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

        // ✅ NUEVO MÉTODO: Usando CreateVolunteerHoursViewModel
        [HttpGet]
        public async Task<IActionResult> CreateHours(int requestId)
        {
            // Verificar permisos y validaciones
            var validationResult = await ValidateRequestAccessAsync(requestId, requireApproved: true);
            if (validationResult.redirectResult != null)
                return validationResult.redirectResult;

            var requestDto = validationResult.request;

            // ✅ REQUERIMIENTO 1: No puede registrar horas si ya cumplió las horas comprometidas
            if (requestDto.RemainingHours <= 0)
            {
                this.SetErrorMessage("Ya has cumplido con todas las horas comprometidas para esta solicitud");
                return RedirectToAction("ManageHours", new { requestId });
            }

            // ✅ USAR NUEVO MODELO CON VALIDACIONES INTEGRADAS
            var model = await _volunteerHoursService.BuildCreateHoursViewModelAsync(requestId, requestDto);

            // ✅ CORREGIDO: ViewBag completo para evitar errores
            SetViewBagRequestInfo(requestDto, false, "", "");

            return View(model);
        }

        // ✅ NUEVO MÉTODO: Crear horas con validaciones mejoradas
        [HttpPost]
        public async Task<IActionResult> CreateHours(CreateVolunteerHoursViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var requestInfo = await _volunteerRequestService.GetRequestByIdAsync(model.VolunteerRequestId);
                if (requestInfo.IsSuccess)
                    SetViewBagRequestInfo(requestInfo.Value, false, "", "");
                return View(model);
            }

            // ✅ VALIDACIÓN PRINCIPAL: Verificar horas restantes ANTES DE TODO
            var hoursValidationResult = await ValidateRemainingHoursBeforeCreate(model.VolunteerRequestId, model.StartTime, model.EndTime);
            if (hoursValidationResult.IsFailure)
            {
                this.SetErrorMessage(hoursValidationResult.Errors);
                var requestInfo = await _volunteerRequestService.GetRequestByIdAsync(model.VolunteerRequestId);
                if (requestInfo.IsSuccess)
                    SetViewBagRequestInfo(requestInfo.Value, false, "", "");
                return View(model);
            }

            // ✅ REQUERIMIENTO 1: Validar horas restantes ANTES de enviar
            if (!model.IsValidHours)
            {
                this.SetErrorMessage(model.ValidationMessage);
                var requestInfo = await _volunteerRequestService.GetRequestByIdAsync(model.VolunteerRequestId);
                if (requestInfo.IsSuccess)
                    SetViewBagRequestInfo(requestInfo.Value, false, "", "");
                return View(model);
            }

            // Validaciones de permisos
            var validationResult = await ValidateRequestAccessAsync(model.VolunteerRequestId, requireApproved: true);
            if (validationResult.redirectResult != null)
                return validationResult.redirectResult;

            var result = await _volunteerHoursService.CreateVolunteerHoursAsync(model);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                SetViewBagRequestInfo(validationResult.request, false, "", "");
                return View(model);
            }

            this.SetSuccessMessage("Horas registradas correctamente. Pendiente de aprobación.");
            return RedirectToAction("ManageHours", new { requestId = model.VolunteerRequestId });
        }

        // ===== MÉTODOS LEGACY MANTENIDOS PARA COMPATIBILIDAD =====
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

            // ✅ CORREGIDO: ViewBag completo para evitar errores
            SetViewBagRequestInfo(requestDto, false, "", "");

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

            // ✅ VALIDACIÓN PRINCIPAL: Verificar horas restantes ANTES DE TODO
            var hoursValidationResult = await ValidateRemainingHoursBeforeCreate(model.RequestId, model.StartTime, model.EndTime);
            if (hoursValidationResult.IsFailure)
            {
                this.SetErrorMessage(hoursValidationResult.Errors);
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
            // ✅ REQUERIMIENTO 2: Verificar que el registro existe ANTES de continuar
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

            // ✅ REQUERIMIENTO 3: Permitir editar horas pendientes o rechazadas
            if (hours.State == VolunteerState.Approved)
            {
                this.SetErrorMessage("No se pueden editar horas ya aprobadas");
                return RedirectToAction("ManageHours", new { requestId = hours.VolunteerRequestId });
            }

            // ✅ MOSTRAR MENSAJE INFORMATIVO SI ESTÁ RECHAZADA
            if (hours.State == VolunteerState.Rejected)
            {
                this.SetInfoMessage($"Editando horas rechazadas. Razón del rechazo: {hours.RejectionReason}");
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

            // ✅ CORREGIDO: ViewBag completo para evitar errores
            SetViewBagRequestInfo(validationResult.request, true, hours.State.ToString(), hours.RejectionReason);

            return View("AddHours", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditHours(AddHoursViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadRequestInfoForEditView(model.Id!.Value);
                return View("AddHours", model);
            }

            // ✅ REQUERIMIENTO 2: Verificar que el registro existe antes de actualizar
            if (!model.Id.HasValue)
            {
                this.SetErrorMessage("ID de registro de horas requerido");
                return RedirectToAction("Index");
            }

            var existingHoursResult = await _volunteerHoursService.GetHoursByIdAsync(model.Id.Value);
            if (existingHoursResult.IsFailure)
            {
                this.SetErrorMessage("Registro de horas no encontrado");
                return RedirectToAction("Index");
            }

            // ✅ VALIDACIÓN PRINCIPAL: Verificar horas restantes excluyendo las horas actuales
            var hoursValidationResult = await ValidateRemainingHoursBeforeUpdate(model.RequestId, model.Id.Value, model.StartTime, model.EndTime);
            if (hoursValidationResult.IsFailure)
            {
                this.SetErrorMessage(hoursValidationResult.Errors);
                await LoadRequestInfoForEditView(model.Id.Value);
                return View("AddHours", model);
            }

            // Verificar permisos
            var validationResult = await ValidateRequestAccessAsync(model.RequestId, requireApproved: true);
            if (validationResult.redirectResult != null)
                return validationResult.redirectResult;

            var result = await _volunteerHoursService.UpdateVolunteerHoursAsync(model.Id.Value, model);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                await LoadRequestInfoForEditView(model.Id.Value);
                return View("AddHours", model);
            }

            this.SetSuccessMessage("Horas actualizadas correctamente");
            return RedirectToAction("ManageHours", new { requestId = model.RequestId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteHours(int hoursId, int requestId)
        {
            // ✅ REQUERIMIENTO 2: Verificar que el registro existe antes de eliminar
            var hoursResult = await _volunteerHoursService.GetHoursByIdAsync(hoursId);
            if (hoursResult.IsFailure)
            {
                this.SetErrorMessage("Registro de horas no encontrado");
                return RedirectToAction("ManageHours", new { requestId });
            }

            // Verificar permisos
            var validationResult = await ValidateRequestAccessAsync(requestId, requireApproved: true);
            if (validationResult.redirectResult != null)
                return validationResult.redirectResult;

            // ✅ REQUERIMIENTO 3: Permitir eliminar horas pendientes o rechazadas
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

        // ✅ NUEVO ENDPOINT: Validar horas con información de horas restantes
        [HttpPost]
        public async Task<JsonResult> ValidateHoursWithRemaining([FromBody] CreateVolunteerHoursViewModel model)
        {
            try
            {
                var result = await _volunteerHoursService.ValidateHoursWithRemainingHoursAsync(model);
                return Json(new
                {
                    isValid = result.IsSuccess,
                    errors = result.Errors,
                    remainingHours = model.RemainingHours,
                    calculatedHours = model.CalculatedHours
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

        // ✅ REQUERIMIENTO 3: Endpoint mejorado para verificar registros existentes
        [HttpGet]
        public async Task<JsonResult> CheckHoursForDate(int requestId, DateTime date)
        {
            try
            {
                var hoursForDate = await _volunteerHoursService.GetHoursByDateRangeAsync(requestId, date, date);

                if (hoursForDate.IsSuccess && hoursForDate.Value?.Any() == true)
                {
                    var existingHour = hoursForDate.Value.First();
                    return Json(new
                    {
                        hasHours = true,
                        state = existingHour.State.ToString(),
                        canReregister = existingHour.State == VolunteerState.Rejected,
                        rejectionReason = existingHour.RejectionReason,
                        message = existingHour.State switch
                        {
                            VolunteerState.Approved => "Ya existe un registro aprobado para esta fecha",
                            VolunteerState.Pending => "Ya existe un registro pendiente para esta fecha",
                            VolunteerState.Rejected => $"Existe un registro rechazado para esta fecha. Puedes registrar nuevas horas (se reemplazará el anterior). Razón del rechazo: {existingHour.RejectionReason}",
                            _ => "Ya existe un registro para esta fecha"
                        }
                    });
                }

                return Json(new { hasHours = false, canReregister = true, message = "" });
            }
            catch
            {
                return Json(new { hasHours = false, canReregister = true, message = "" });
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

        // ✅ CORREGIDO: Método para establecer ViewBag completo
        private void SetViewBagRequestInfo(VolunteerRequestDto request, bool isEditing, string currentState, string rejectionReason)
        {
            ViewBag.RequestInfo = new
            {
                Institution = request.Institution,
                RemainingHours = request.RemainingHours,
                TotalHours = request.Hours,
                ApprovedHours = request.HoursWorked,
                IsEditing = isEditing,
                CurrentState = currentState,
                RejectionReason = rejectionReason
            };
        }

        // ✅ CORREGIDO: Método legacy actualizado
        private async Task LoadRequestInfoForView(int requestId)
        {
            var requestInfo = await _volunteerRequestService.GetRequestByIdAsync(requestId);
            if (requestInfo.IsSuccess)
            {
                SetViewBagRequestInfo(requestInfo.Value, false, "", "");
            }
        }

        // ✅ NUEVO: Método para cargar información al editar
        private async Task LoadRequestInfoForEditView(int hoursId)
        {
            var hoursResult = await _volunteerHoursService.GetHoursByIdAsync(hoursId);
            if (hoursResult.IsSuccess)
            {
                var requestInfo = await _volunteerRequestService.GetRequestByIdAsync(hoursResult.Value.VolunteerRequestId);
                if (requestInfo.IsSuccess)
                {
                    SetViewBagRequestInfo(requestInfo.Value, true, hoursResult.Value.State.ToString(), hoursResult.Value.RejectionReason);
                }
            }
        }

        // ✅ NUEVA VALIDACIÓN PRINCIPAL: Verificar horas restantes antes de crear
        private async Task<Result> ValidateRemainingHoursBeforeCreate(int requestId, TimeSpan startTime, TimeSpan endTime)
        {
            try
            {
                Console.WriteLine($"DEBUG CONTROLLER: Validando horas restantes para crear - RequestId: {requestId}");

                // 1. Obtener información de la solicitud principal
                var requestResult = await _volunteerRequestService.GetRequestByIdAsync(requestId);
                if (requestResult.IsFailure)
                {
                    Console.WriteLine("DEBUG CONTROLLER: Solicitud no encontrada");
                    return Result.Failure("Solicitud no encontrada");
                }

                var request = requestResult.Value;
                Console.WriteLine($"DEBUG CONTROLLER: Solicitud encontrada - Horas propuestas: {request.Hours}");

                // 2. Obtener TODAS las horas registradas para esta solicitud (todos los estados)
                var allHoursResult = await _volunteerHoursService.GetHoursByRequestIdAsync(requestId);
                if (allHoursResult.IsFailure)
                {
                    Console.WriteLine("DEBUG CONTROLLER: Error al obtener horas existentes");
                    return Result.Failure("Error al verificar horas existentes");
                }

                var allRegisteredHours = allHoursResult.Value ?? new List<VolunteerHoursDto>();
                Console.WriteLine($"DEBUG CONTROLLER: Total de registros de horas encontrados: {allRegisteredHours.Count}");

                // 3. Sumar TODAS las horas registradas (Pending + Approved - rejected)
                var totalRegisteredHours = allRegisteredHours
                    .Where(h => h.State != VolunteerState.Rejected)
                    .Sum(h => h.TotalHours);

                // 4. Calcular horas de la nueva solicitud
                var newHours = (decimal)(endTime - startTime).TotalHours;
                Console.WriteLine($"DEBUG CONTROLLER: Nuevas horas a registrar: {newHours}");

                // 5. Calcular horas restantes disponibles
                var remainingHours = request.Hours - totalRegisteredHours;
                Console.WriteLine($"DEBUG CONTROLLER: Horas restantes disponibles: {remainingHours}");

                // 6. VALIDACIÓN PRINCIPAL: tiempo_solicitado <= tiempo_restante
                if (newHours > remainingHours)
                {
                    var errorMessage = $"No puedes registrar {newHours:F1} horas. Ya tienes {totalRegisteredHours:F1} horas registradas de las {request.Hours} horas propuestas. Solo quedan {remainingHours:F1} horas disponibles.";
                    Console.WriteLine($"DEBUG CONTROLLER: VALIDACIÓN FALLIDA - {errorMessage}");
                    return Result.Failure(errorMessage);
                }

                // 7. Validación adicional: no permitir si ya completó las horas
                if (remainingHours <= 0)
                {
                    var errorMessage = "Ya has registrado todas las horas comprometidas para esta solicitud.";
                    Console.WriteLine($"DEBUG CONTROLLER: VALIDACIÓN FALLIDA - {errorMessage}");
                    return Result.Failure(errorMessage);
                }

                Console.WriteLine($"DEBUG CONTROLLER: VALIDACIÓN EXITOSA - Puede registrar {newHours:F1} horas");
                return Result.Success();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG CONTROLLER: Error en validación de horas restantes: {ex.Message}");
                return Result.Failure($"Error al validar horas restantes: {ex.Message}");
            }
        }

        // ✅ NUEVA VALIDACIÓN PARA EDICIÓN: Verificar horas restantes excluyendo las horas actuales
        private async Task<Result> ValidateRemainingHoursBeforeUpdate(int requestId, int currentHoursId, TimeSpan startTime, TimeSpan endTime)
        {
            try
            {
                Console.WriteLine($"DEBUG CONTROLLER: Validando horas restantes para actualizar - RequestId: {requestId}, HoursId: {currentHoursId}");

                // 1. Obtener información de la solicitud principal
                var requestResult = await _volunteerRequestService.GetRequestByIdAsync(requestId);
                if (requestResult.IsFailure)
                {
                    return Result.Failure("Solicitud no encontrada");
                }

                var request = requestResult.Value;

                // 2. Obtener TODAS las horas registradas para esta solicitud
                var allHoursResult = await _volunteerHoursService.GetHoursByRequestIdAsync(requestId);
                if (allHoursResult.IsFailure)
                {
                    return Result.Failure("Error al verificar horas existentes");
                }

                var allRegisteredHours = allHoursResult.Value ?? new List<VolunteerHoursDto>();

                // 3. Sumar TODAS las horas EXCEPTO las del registro que se está editando
                var totalRegisteredHours = allRegisteredHours 
                    .Where(h => h.Id != currentHoursId && h.State != VolunteerState.Rejected)
                    .Sum(h => h.TotalHours);
                Console.WriteLine($"DEBUG CONTROLLER: Total horas registradas (excluyendo actual): {totalRegisteredHours}");

                // 4. Calcular nuevas horas a registrar
                var newHours = (decimal)(endTime - startTime).TotalHours;
                Console.WriteLine($"DEBUG CONTROLLER: Nuevas horas a registrar: {newHours}");

                // 5. Calcular horas restantes disponibles
                var remainingHours = request.Hours - totalRegisteredHours;
                Console.WriteLine($"DEBUG CONTROLLER: Horas restantes disponibles para edición: {remainingHours}");

                // 6. VALIDACIÓN PRINCIPAL: tiempo_solicitado <= tiempo_restante
                if (newHours > remainingHours)
                {
                    var errorMessage = $"No puedes registrar {newHours:F1} horas. Ya tienes {totalRegisteredHours:F1} horas registradas en otros días de las {request.Hours} horas propuestas. Solo quedan {remainingHours:F1} horas disponibles.";
                    Console.WriteLine($"DEBUG CONTROLLER: VALIDACIÓN FALLIDA EN EDICIÓN - {errorMessage}");
                    return Result.Failure(errorMessage);
                }

                Console.WriteLine($"DEBUG CONTROLLER: VALIDACIÓN DE EDICIÓN EXITOSA - Puede actualizar a {newHours:F1} horas");
                return Result.Success();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG CONTROLLER: Error en validación de horas para edición: {ex.Message}");
                return Result.Failure($"Error al validar horas restantes: {ex.Message}");
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
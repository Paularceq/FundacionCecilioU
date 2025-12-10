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
            var request = await _volunteerRequestService.GetRequestByIdAsync(requestId);
            if (request.IsFailure)
            {
                this.SetErrorMessage("Solicitud no encontrada");
                return RedirectToAction("Index");
            }

            var requestDto = request.Value;

            if (requestDto.VolunteerId != GetCurrentUserId())
            {
                this.SetErrorMessage("No tienes permisos para gestionar esta solicitud");
                return RedirectToAction("Index");
            }

            if (requestDto.State != VolunteerState.Approved)
            {
                this.SetErrorMessage("Solo se pueden gestionar horas para solicitudes aprobadas");
                return RedirectToAction("Index");
            }

            var viewModel = await _volunteerHoursService.BuildManageHoursViewModelAsync(requestId, requestDto);

            return View(viewModel);
        }

        // ===== REGISTRO DE HORAS =====
        [HttpGet]
        public async Task<IActionResult> AddHours(int requestId)
        {
            // PASO 1: Validar permisos
            var validationResult = await ValidateRequestAccessAsync(requestId, requireApproved: true);
            if (validationResult.redirectResult != null)
                return validationResult.redirectResult;

            var requestDto = validationResult.request;

            // PASO 2: Validar horas restantes
            if (requestDto.RemainingHours <= 0)
            {
                this.SetErrorMessage("Ya has cumplido con todas las horas comprometidas para esta solicitud");
                return RedirectToAction("ManageHours", new { requestId });
            }

            // PASO 3: Crear modelo
            var model = new AddHoursViewModel
            {
                RequestId = requestId,
                Date = DateTime.Today,
                StartTime = TimeSpan.FromHours(8),
                EndTime = TimeSpan.FromHours(17)
            };

            SetViewBagRequestInfo(requestDto, false, "", "");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddHours(AddHoursViewModel model)
        {
            // PASO 1: Validar ModelState
            if (!ModelState.IsValid)
            {
                await LoadRequestInfoForView(model.RequestId);
                return View(model);
            }

            // PASO 2: Validar permisos y acceso
            var validationResult = await ValidateRequestAccessAsync(model.RequestId, requireApproved: true);
            if (validationResult.redirectResult != null)
                return validationResult.redirectResult;

            // PASO 3: Validar que hora fin sea mayor que hora inicio
            if (model.EndTime <= model.StartTime)
            {
                ModelState.AddModelError("EndTime", "La hora de fin debe ser posterior a la hora de inicio");
                await LoadRequestInfoForView(model.RequestId);
                return View(model);
            }

            // PASO 4: Validar rango de horas (1-8 por día)
            var totalHours = (decimal)(model.EndTime - model.StartTime).TotalHours;
            if (totalHours < 1 || totalHours > 8)
            {
                ModelState.AddModelError("", "Debes registrar entre 1 y 8 horas por día");
                await LoadRequestInfoForView(model.RequestId);
                return View(model);
            }

            // PASO 5: Validar que no haya traslapes de horarios en el mismo día
            var overlapValidation = await ValidateTimeOverlap(
                model.RequestId,
                model.Date,
                model.StartTime,
                model.EndTime,
                null
            );

            if (overlapValidation.IsFailure)
            {
                ModelState.AddModelError("StartTime", overlapValidation.Errors.FirstOrDefault() ?? "Traslape de horarios");
                await LoadRequestInfoForView(model.RequestId);
                return View(model);
            }

            // PASO 6: Validar horas restantes disponibles
            var hoursValidationResult = await ValidateRemainingHoursBeforeCreate(model.RequestId, model.StartTime, model.EndTime);
            if (hoursValidationResult.IsFailure)
            {
                this.SetErrorMessage(hoursValidationResult.Errors);
                await LoadRequestInfoForView(model.RequestId);
                return View(model);
            }

            // PASO 7: Crear el registro
            var result = await _volunteerHoursService.CreateVolunteerHoursAsync(model);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                await LoadRequestInfoForView(model.RequestId);
                return View(model);
            }

            this.SetSuccessMessage("Horas registradas correctamente. Pendientes de aprobación.");
            return RedirectToAction("ManageHours", new { requestId = model.RequestId });
        }

        [HttpGet]
        public async Task<IActionResult> EditHours(int hoursId)
        {
            // PASO 1: Verificar que el registro existe
            var hoursResult = await _volunteerHoursService.GetHoursByIdAsync(hoursId);
            if (hoursResult.IsFailure)
            {
                this.SetErrorMessage("Registro de horas no encontrado");
                return RedirectToAction("Index");
            }

            var hours = hoursResult.Value;

            // PASO 2: Validar permisos
            var validationResult = await ValidateRequestAccessAsync(hours.VolunteerRequestId, requireApproved: true);
            if (validationResult.redirectResult != null)
                return validationResult.redirectResult;

            // PASO 3: Validar que no esté aprobado
            if (hours.State == VolunteerState.Approved)
            {
                this.SetErrorMessage("No se pueden editar horas ya aprobadas");
                return RedirectToAction("ManageHours", new { requestId = hours.VolunteerRequestId });
            }

            // PASO 4: Mostrar mensaje si está rechazado
            if (hours.State == VolunteerState.Rejected)
            {
                this.SetInfoMessage($"Editando horas rechazadas. Razón del rechazo: {hours.RejectionReason}");
            }

            // PASO 5: Crear modelo
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

            SetViewBagRequestInfo(validationResult.request, true, hours.State.ToString(), hours.RejectionReason);
            return View("AddHours", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditHours(AddHoursViewModel model)
        {
            // PASO 1: Validar ModelState
            if (!ModelState.IsValid)
            {
                await LoadRequestInfoForEditView(model.Id!.Value);
                return View("AddHours", model);
            }

            // PASO 2: Verificar que tiene ID
            if (!model.Id.HasValue)
            {
                this.SetErrorMessage("ID de registro de horas requerido");
                return RedirectToAction("Index");
            }

            // PASO 3: Verificar que el registro existe
            var existingHoursResult = await _volunteerHoursService.GetHoursByIdAsync(model.Id.Value);
            if (existingHoursResult.IsFailure)
            {
                this.SetErrorMessage("Registro de horas no encontrado");
                return RedirectToAction("Index");
            }

            var currentHours = existingHoursResult.Value;

            // PASO 4: Validar permisos
            var validationResult = await ValidateRequestAccessAsync(model.RequestId, requireApproved: true);
            if (validationResult.redirectResult != null)
                return validationResult.redirectResult;

            // PASO 5: Validar que no esté aprobado
            if (currentHours.State == VolunteerState.Approved)
            {
                this.SetErrorMessage("No se pueden editar horas ya aprobadas");
                return RedirectToAction("ManageHours", new { requestId = model.RequestId });
            }

            // PASO 6: Validar que hora fin sea mayor que hora inicio
            if (model.EndTime <= model.StartTime)
            {
                ModelState.AddModelError("EndTime", "La hora de fin debe ser posterior a la hora de inicio");
                await LoadRequestInfoForEditView(model.Id.Value);
                return View("AddHours", model);
            }

            // PASO 7: Validar rango de horas (1-8 por día)
            var totalHours = (decimal)(model.EndTime - model.StartTime).TotalHours;
            if (totalHours < 1 || totalHours > 8)
            {
                ModelState.AddModelError("", "Debes registrar entre 1 y 8 horas por día");
                await LoadRequestInfoForEditView(model.Id.Value);
                return View("AddHours", model);
            }

            // PASO 8: Validar que no haya traslapes (excluyendo el registro actual)
            var overlapValidation = await ValidateTimeOverlap(
                model.RequestId,
                model.Date,
                model.StartTime,
                model.EndTime,
                model.Id.Value
            );

            if (overlapValidation.IsFailure)
            {
                ModelState.AddModelError("StartTime", overlapValidation.Errors.FirstOrDefault() ?? "Traslape de horarios");
                await LoadRequestInfoForEditView(model.Id.Value);
                return View("AddHours", model);
            }

            // PASO 9: Validar horas restantes (excluyendo el registro actual)
            var hoursValidationResult = await ValidateRemainingHoursBeforeUpdate(
                model.RequestId,
                model.Id.Value,
                model.StartTime,
                model.EndTime
            );

            if (hoursValidationResult.IsFailure)
            {
                this.SetErrorMessage(hoursValidationResult.Errors);
                await LoadRequestInfoForEditView(model.Id.Value);
                return View("AddHours", model);
            }

            // PASO 10: Actualizar el registro
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
            // PASO 1: Verificar que el registro existe
            var hoursResult = await _volunteerHoursService.GetHoursByIdAsync(hoursId);
            if (hoursResult.IsFailure)
            {
                this.SetErrorMessage("Registro de horas no encontrado");
                return RedirectToAction("ManageHours", new { requestId });
            }

            // PASO 2: Validar permisos
            var validationResult = await ValidateRequestAccessAsync(requestId, requireApproved: true);
            if (validationResult.redirectResult != null)
                return validationResult.redirectResult;

            // PASO 3: Validar que no esté aprobado
            if (hoursResult.Value.State == VolunteerState.Approved)
            {
                this.SetErrorMessage("No se pueden eliminar horas ya aprobadas");
                return RedirectToAction("ManageHours", new { requestId });
            }

            // PASO 4: Eliminar el registro
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
            var errores = new List<string>();

            // 1. Validar ModelState básico
            if (model.EndTime <= model.StartTime)
            {
                errores.Add("La hora de fin debe ser posterior a la hora de inicio.");
            }

            var totalHours = (decimal)(model.EndTime - model.StartTime).TotalHours;
            if (totalHours < 1 || totalHours > 8)
            {
                errores.Add("Debes registrar entre 1 y 8 horas por día.");
            }

            if (!errores.Any())
            {
                // 2. Validar traslapes (usando el mismo método del controlador)
                var overlapValidation = await ValidateTimeOverlap(
                    model.RequestId,
                    model.Date,
                    model.StartTime,
                    model.EndTime,
                    model.Id // null si es crear, valor si es editar
                );

                if (overlapValidation.IsFailure)
                {
                    errores.AddRange(overlapValidation.Errors);
                }
            }

            if (!errores.Any())
            {
                // 3. Validar horas restantes según si es crear o editar
                Result remainingValidation;

                if (model.Id.HasValue)
                {
                    remainingValidation = await ValidateRemainingHoursBeforeUpdate(
                        model.RequestId,
                        model.Id.Value,
                        model.StartTime,
                        model.EndTime
                    );
                }
                else
                {
                    remainingValidation = await ValidateRemainingHoursBeforeCreate(
                        model.RequestId,
                        model.StartTime,
                        model.EndTime
                    );
                }

                if (remainingValidation.IsFailure)
                {
                    errores.AddRange(remainingValidation.Errors);
                }
            }

            return Json(new
            {
                isValid = !errores.Any(),
                errors = errores
            });
        }


        [HttpGet]
        public async Task<JsonResult> CheckHoursForDate(int requestId, DateTime date)
        {
            try
            {
                var hoursForDate = await _volunteerHoursService.GetHoursByDateRangeAsync(requestId, date, date);

                if (hoursForDate.IsSuccess && hoursForDate.Value?.Any() == true)
                {
                    var existingHours = hoursForDate.Value.Where(h => h.State != VolunteerState.Rejected).ToList();

                    if (existingHours.Any())
                    {
                        var hoursList = string.Join(", ", existingHours.Select(h => $"{h.StartTime:hh\\:mm}-{h.EndTime:hh\\:mm}"));

                        return Json(new
                        {
                            hasHours = true,
                            message = $"Ya tienes registros para esta fecha: {hoursList}. Los horarios no pueden traslaparse.",
                            canReregister = true
                        });
                    }
                }

                return Json(new { hasHours = false, canReregister = true, message = "" });
            }
            catch
            {
                return Json(new { hasHours = false, canReregister = true, message = "" });
            }
        }

        // ===== MÉTODOS DE VALIDACIÓN =====

        /// <summary>
        /// VALIDACIÓN: Verificar que no haya traslape de horarios en el mismo día
        /// Bloquea:
        ///   - Doble registro exacto (misma fecha, misma hora inicio/fin)
        ///   - Traslapes parciales o totales
        /// Permite:
        ///   - Horas consecutivas (08:00-09:00 y 09:00-10:00)
        ///   - Horas distintas en el mismo día sin traslape (08:00-09:00 y 13:00-14:00)
        /// </summary>
        private async Task<Result> ValidateTimeOverlap(
            int requestId,
            DateTime date,
            TimeSpan startTime,
            TimeSpan endTime,
            int? excludeHoursId)
        {
            try
            {
                var existingHoursForDateResult =
                    await _volunteerHoursService.GetHoursByDateRangeAsync(requestId, date, date);

                if (!existingHoursForDateResult.IsSuccess || existingHoursForDateResult.Value == null)
                    return Result.Success();

                var existingHoursForDate = existingHoursForDateResult.Value;

                foreach (var existingHour in existingHoursForDate)
                {
                    // Ignorar registros rechazados
                    if (existingHour.State == VolunteerState.Rejected)
                        continue;

                    // Si estamos editando, excluir el registro actual
                    if (excludeHoursId.HasValue && existingHour.Id == excludeHoursId.Value)
                        continue;

                    // Asegurar misma fecha
                    if (date.Date != existingHour.Date.Date)
                        continue;

                    // 1) Duplicado EXACTO: misma fecha + mismo rango
                    bool esMismoRangoExacto =
                        existingHour.StartTime == startTime &&
                        existingHour.EndTime == endTime;

                    if (esMismoRangoExacto)
                    {
                        return Result.Failure(
                            $"Ya tienes un registro con el mismo horario " +
                            $"({existingHour.StartTime:hh\\:mm} - {existingHour.EndTime:hh\\:mm}) en esta fecha.");
                    }

                    // 2) Traslape, permitiendo solo consecutivos
                    //
                    // Se consideran traslapados si:
                    //   inicioNuevo < finExistente  Y  finNuevo > inicioExistente
                    // PERO NO si son consecutivos:
                    //   inicioNuevo == finExistente  (08-09, 09-10)
                    //   O finNuevo == inicioExistente
                    bool hayTraslape =
                        (startTime < existingHour.EndTime && endTime > existingHour.StartTime) &&
                        !(startTime == existingHour.EndTime || endTime == existingHour.StartTime);

                    if (hayTraslape)
                    {
                        return Result.Failure(
                            $"El horario {startTime:hh\\:mm} - {endTime:hh\\:mm} se traslapa con un registro existente " +
                            $"({existingHour.StartTime:hh\\:mm} - {existingHour.EndTime:hh\\:mm}). Estado: {existingHour.State}");
                    }
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error al validar traslape de horarios: {ex.Message}");
            }
        }


        /// <summary>
        /// VALIDACIÓN: Verificar horas restantes antes de crear
        /// </summary>
        private async Task<Result> ValidateRemainingHoursBeforeCreate(int requestId, TimeSpan startTime, TimeSpan endTime)
        {
            try
            {
                var requestResult = await _volunteerRequestService.GetRequestByIdAsync(requestId);
                if (requestResult.IsFailure)
                {
                    return Result.Failure("Solicitud no encontrada");
                }

                var request = requestResult.Value;

                var allHoursResult = await _volunteerHoursService.GetHoursByRequestIdAsync(requestId);
                if (allHoursResult.IsFailure)
                {
                    return Result.Failure("Error al verificar horas existentes");
                }

                var allRegisteredHours = allHoursResult.Value ?? new List<VolunteerHoursDto>();

                var totalRegisteredHours = allRegisteredHours
                    .Where(h => h.State != VolunteerState.Rejected)
                    .Sum(h => h.TotalHours);

                var newHours = (decimal)(endTime - startTime).TotalHours;
                var remainingHours = request.Hours - totalRegisteredHours;

                if (remainingHours <= 0)
                {
                    return Result.Failure("Ya has registrado todas las horas comprometidas para esta solicitud.");
                }

                if (newHours > remainingHours)
                {
                    return Result.Failure(
                        $"No puedes registrar {newHours:F1} horas. " +
                        $"Ya tienes {totalRegisteredHours:F1} horas registradas de las {request.Hours} propuestas. " +
                        $"Solo quedan {remainingHours:F1} horas disponibles."
                    );
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error al validar horas restantes: {ex.Message}");
            }
        }

        /// <summary>
        /// VALIDACIÓN: Verificar horas restantes antes de actualizar (excluyendo el registro actual)
        /// </summary>
        private async Task<Result> ValidateRemainingHoursBeforeUpdate(int requestId, int currentHoursId, TimeSpan startTime, TimeSpan endTime)
        {
            try
            {
                var requestResult = await _volunteerRequestService.GetRequestByIdAsync(requestId);
                if (requestResult.IsFailure)
                {
                    return Result.Failure("Solicitud no encontrada");
                }

                var request = requestResult.Value;

                var allHoursResult = await _volunteerHoursService.GetHoursByRequestIdAsync(requestId);
                if (allHoursResult.IsFailure)
                {
                    return Result.Failure("Error al verificar horas existentes");
                }

                var allRegisteredHours = allHoursResult.Value ?? new List<VolunteerHoursDto>();

                // Excluir el registro actual que se está editando
                var totalRegisteredHours = allRegisteredHours
                    .Where(h => h.Id != currentHoursId && h.State != VolunteerState.Rejected)
                    .Sum(h => h.TotalHours);

                var newHours = (decimal)(endTime - startTime).TotalHours;
                var remainingHours = request.Hours - totalRegisteredHours;

                if (newHours > remainingHours)
                {
                    return Result.Failure(
                        $"No puedes registrar {newHours:F1} horas. " +
                        $"Ya tienes {totalRegisteredHours:F1} horas registradas en otros días. " +
                        $"Solo quedan {remainingHours:F1} horas disponibles."
                    );
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error al validar horas restantes: {ex.Message}");
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

            if (request.VolunteerId != GetCurrentUserId())
            {
                this.SetErrorMessage("No tienes permisos para acceder a esta solicitud");
                return (RedirectToAction("Index"), null!);
            }

            if (requireApproved && request.State != VolunteerState.Approved)
            {
                this.SetErrorMessage("Solo se pueden gestionar horas para solicitudes aprobadas");
                return (RedirectToAction("Index"), null!);
            }

            return (null, request);
        }

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

        private async Task LoadRequestInfoForView(int requestId)
        {
            var requestInfo = await _volunteerRequestService.GetRequestByIdAsync(requestId);
            if (requestInfo.IsSuccess)
            {
                SetViewBagRequestInfo(requestInfo.Value, false, "", "");
            }
        }

        private async Task LoadRequestInfoForEditView(int hoursId)
        {
            var hoursResult = await _volunteerHoursService.GetHoursByIdAsync(hoursId);
            if (hoursResult.IsSuccess)
            {
                var requestInfo = await _volunteerRequestService.GetRequestByIdAsync(hoursResult.Value.VolunteerRequestId);
                if (requestInfo.IsSuccess)
                {
                    SetViewBagRequestInfo(
                        requestInfo.Value,
                        true,
                        hoursResult.Value.State.ToString(),
                        hoursResult.Value.RejectionReason
                    );
                }
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

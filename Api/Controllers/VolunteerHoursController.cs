using Api.Abstractions.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Constants;
using Shared.Dtos.Volunteer;
using System.Security.Claims;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VolunteerHoursController : ControllerBase
    {
        private readonly IVolunteerRequestService _volunteerRequestService;

        public VolunteerHoursController(IVolunteerRequestService volunteerRequestService)
        {
            _volunteerRequestService = volunteerRequestService;
        }

        // ===== MÉTODO AUXILIAR PARA EXTRAER ERRORES DE MODELSTATE =====
        private List<string> GetModelErrors()
        {
            return ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Error de validación" : e.ErrorMessage)
                .ToList();
        }

        // ===== CRUD DE HORAS =====
        [HttpPost]
        public async Task<IActionResult> CreateVolunteerHours([FromBody] CreateVolunteerHoursDto dto)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = GetModelErrors();
                return BadRequest(modelErrors);
            }

            var result = await _volunteerRequestService.CreateVolunteerHoursAsync(dto);
            if (result.IsFailure)
                return BadRequest(result.Errors); // <-- directamente lista

            return Ok(new { message = "Horas registradas correctamente" });
        }

        [HttpGet("request/{requestId}")]
        public async Task<IActionResult> GetHoursByRequestId(int requestId)
        {
            var result = await _volunteerRequestService.GetHoursByRequestIdAsync(requestId);
            if (result.IsFailure)
                return BadRequest(result.Errors);

            return Ok(result.Value);
        }

        [HttpGet("request/{requestId}/date-range")]
        public async Task<IActionResult> GetHoursByDateRange(int requestId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
            {
                var errors = new List<string> { "La fecha de inicio no puede ser mayor a la fecha de fin" };
                return BadRequest(errors);
            }

            var result = await _volunteerRequestService.GetHoursByDateRangeAsync(requestId, startDate, endDate);
            if (result.IsFailure)
                return BadRequest(result.Errors);

            return Ok(result.Value);
        }

        [HttpPut("{hoursId}")]
        public async Task<IActionResult> UpdateVolunteerHours(int hoursId, [FromBody] CreateVolunteerHoursDto dto)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = GetModelErrors();
                return BadRequest(modelErrors);
            }

            // Verificar que exista
            var existingHoursResult = await _volunteerRequestService.GetVolunteerHoursByIdAsync(hoursId);
            if (existingHoursResult.IsFailure)
                return BadRequest(existingHoursResult.Errors);

            var result = await _volunteerRequestService.UpdateVolunteerHoursAsync(hoursId, dto);
            if (result.IsFailure)
                return BadRequest(result.Errors);

            return Ok(new { message = "Horas actualizadas correctamente" });
        }

        [HttpDelete("{hoursId}")]
        public async Task<IActionResult> DeleteVolunteerHours(int hoursId)
        {
            var existingHoursResult = await _volunteerRequestService.GetVolunteerHoursByIdAsync(hoursId);
            if (existingHoursResult.IsFailure)
                return BadRequest(existingHoursResult.Errors);

            var result = await _volunteerRequestService.DeleteVolunteerHoursAsync(hoursId);
            if (result.IsFailure)
                return BadRequest(result.Errors);

            return Ok(new { message = "Horas eliminadas correctamente" });
        }

        // ===== NUEVO ENDPOINT AGREGADO =====
        [HttpGet("{hoursId}")]
        public async Task<IActionResult> GetVolunteerHoursById(int hoursId)
        {
            var result = await _volunteerRequestService.GetVolunteerHoursByIdAsync(hoursId);
            if (result.IsFailure)
                return BadRequest(result.Errors);

            return Ok(result.Value);
        }

        // ===== APROBACIÓN DE HORAS =====
        [HttpPost("{hoursId}/approve")]
        [Authorize(Roles = Roles.AdminSistema)]
        public async Task<IActionResult> ApproveHours(int hoursId, [FromBody] ApproveHoursRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = GetModelErrors();
                return BadRequest(modelErrors);
            }

            // Obtener approverId del usuario autenticado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int authenticatedUserId))
            {
                dto.ApproverId = authenticatedUserId;
            }

            var approveDto = new ApproveRejectHoursDto
            {
                HoursId = hoursId,
                IsApproved = true,
                ApproverId = dto.ApproverId,
                ApproverName = dto.ApproverName,
                RejectionReason = null
            };

            var result = await _volunteerRequestService.ApproveHoursAsync(approveDto);
            if (result.IsFailure)
                return BadRequest(result.Errors);

            return Ok(new { message = "Horas aprobadas correctamente" });
        }

        [HttpPost("{hoursId}/reject")]
        [Authorize(Roles = Roles.AdminSistema)]
        public async Task<IActionResult> RejectHours(int hoursId, [FromBody] RejectHoursRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = GetModelErrors();
                return BadRequest(modelErrors);
            }

            if (string.IsNullOrWhiteSpace(dto.RejectionReason))
            {
                return BadRequest(new List<string> { "Debe proporcionar una razón para el rechazo" });
            }

            // Obtener approverId del usuario autenticado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int authenticatedUserId))
            {
                dto.ApproverId = authenticatedUserId;
            }

            var rejectDto = new ApproveRejectHoursDto
            {
                HoursId = hoursId,
                IsApproved = false,
                ApproverId = dto.ApproverId,
                ApproverName = dto.ApproverName,
                RejectionReason = dto.RejectionReason
            };

            var result = await _volunteerRequestService.RejectHoursAsync(rejectDto);
            if (result.IsFailure)
                return BadRequest(result.Errors);

            return Ok(new { message = "Horas rechazadas correctamente" });
        }

        [HttpGet("pending")]
        [Authorize(Roles = Roles.AdminSistema)]
        public async Task<IActionResult> GetPendingHours()
        {
            try
            {
                var hours = await _volunteerRequestService.GetPendingHoursAsync();
                return Ok(hours);
            }
            catch (Exception ex)
            {
                return BadRequest(new List<string> { $"Error al obtener horas pendientes: {ex.Message}" });
            }
        }

        // ===== VALIDACIONES =====
        [HttpPost("validate")]
        public async Task<IActionResult> ValidateHours([FromBody] CreateVolunteerHoursDto dto)
        {
            // Aquí NO devolvemos List<string> directamente porque
            // el cliente espera { isValid, errors }
            if (!ModelState.IsValid)
            {
                var modelErrors = GetModelErrors();
                return Ok(new
                {
                    isValid = false,
                    errors = modelErrors
                });
            }

            var result = await _volunteerRequestService.ValidateHoursAsync(dto);

            return Ok(new
            {
                isValid = result.IsSuccess,
                errors = result.Errors
            });
        }
    }
}

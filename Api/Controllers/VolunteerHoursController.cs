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

        // ===== CRUD DE HORAS =====
        [HttpPost]
        public async Task<IActionResult> CreateVolunteerHours([FromBody] CreateVolunteerHoursDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // ✅ REQUERIMIENTO 1: Las validaciones de horas restantes ya están en el Service
            // ✅ REQUERIMIENTO 3: El manejo de re-registro después de rechazo ya está en el Service
            var result = await _volunteerRequestService.CreateVolunteerHoursAsync(dto);
            if (result.IsFailure)
                return BadRequest(new { errors = result.Errors });

            return Ok(new { message = "Horas registradas correctamente" });
        }

        [HttpGet("request/{requestId}")]
        public async Task<IActionResult> GetHoursByRequestId(int requestId)
        {
            var result = await _volunteerRequestService.GetHoursByRequestIdAsync(requestId);
            if (result.IsFailure)
                return BadRequest(new { errors = result.Errors });

            return Ok(result.Value);
        }

        [HttpGet("request/{requestId}/date-range")]
        public async Task<IActionResult> GetHoursByDateRange(int requestId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
                return BadRequest(new { errors = new[] { "La fecha de inicio no puede ser mayor a la fecha de fin" } });

            var result = await _volunteerRequestService.GetHoursByDateRangeAsync(requestId, startDate, endDate);
            if (result.IsFailure)
                return BadRequest(new { errors = result.Errors });

            return Ok(result.Value);
        }

        [HttpPut("{hoursId}")]
        public async Task<IActionResult> UpdateVolunteerHours(int hoursId, [FromBody] CreateVolunteerHoursDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // ✅ REQUERIMIENTO 2: Verificar que el registro existe antes de actualizar
            var existingHoursResult = await _volunteerRequestService.GetVolunteerHoursByIdAsync(hoursId);
            if (existingHoursResult.IsFailure)
                return BadRequest(new { errors = existingHoursResult.Errors });

            // ✅ REQUERIMIENTO 1: Las validaciones de horas restantes ya están en el Service
            // ✅ REQUERIMIENTO 3: El manejo de conflictos con rechazos ya está en el Service
            var result = await _volunteerRequestService.UpdateVolunteerHoursAsync(hoursId, dto);
            if (result.IsFailure)
                return BadRequest(new { errors = result.Errors });

            return Ok(new { message = "Horas actualizadas correctamente" });
        }

        [HttpDelete("{hoursId}")]
        public async Task<IActionResult> DeleteVolunteerHours(int hoursId)
        {
            // ✅ REQUERIMIENTO 2: Verificar que el registro existe antes de eliminar
            var existingHoursResult = await _volunteerRequestService.GetVolunteerHoursByIdAsync(hoursId);
            if (existingHoursResult.IsFailure)
                return BadRequest(new { errors = existingHoursResult.Errors });

            var result = await _volunteerRequestService.DeleteVolunteerHoursAsync(hoursId);
            if (result.IsFailure)
                return BadRequest(new { errors = result.Errors });

            return Ok(new { message = "Horas eliminadas correctamente" });
        }

        // ===== NUEVO ENDPOINT AGREGADO =====
        [HttpGet("{hoursId}")]
        public async Task<IActionResult> GetVolunteerHoursById(int hoursId)
        {
            var result = await _volunteerRequestService.GetVolunteerHoursByIdAsync(hoursId);
            if (result.IsFailure)
                return BadRequest(new { errors = result.Errors });

            return Ok(result.Value);
        }

        // ===== APROBACIÓN DE HORAS =====
        [HttpPost("{hoursId}/approve")]
        [Authorize(Roles = Roles.AdminSistema)]
        public async Task<IActionResult> ApproveHours(int hoursId, [FromBody] ApproveHoursRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Obtener approverId del usuario autenticado (más seguro)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int authenticatedUserId))
            {
                dto.ApproverId = authenticatedUserId; // Sobrescribir por seguridad
            }

            // Crear el DTO que espera el servicio
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
                return BadRequest(new { errors = result.Errors });

            return Ok(new { message = "Horas aprobadas correctamente" });
        }

        [HttpPost("{hoursId}/reject")]
        [Authorize(Roles = Roles.AdminSistema)]
        public async Task<IActionResult> RejectHours(int hoursId, [FromBody] RejectHoursRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(dto.RejectionReason))
                return BadRequest(new { errors = new[] { "Debe proporcionar una razón para el rechazo" } });

            // Obtener approverId del usuario autenticado (más seguro)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int authenticatedUserId))
            {
                dto.ApproverId = authenticatedUserId; // Sobrescribir por seguridad
            }

            // Crear el DTO que espera el servicio
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
                return BadRequest(new { errors = result.Errors });

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
                return BadRequest(new { errors = new[] { $"Error al obtener horas pendientes: {ex.Message}" } });
            }
        }

        // ===== VALIDACIONES =====
        [HttpPost("validate")]
        public async Task<IActionResult> ValidateHours([FromBody] CreateVolunteerHoursDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _volunteerRequestService.ValidateHoursAsync(dto);
            return Ok(new
            {
                isValid = result.IsSuccess,
                errors = result.Errors
            });
        }
    }
}
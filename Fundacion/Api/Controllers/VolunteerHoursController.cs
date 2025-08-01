using Api.Abstractions.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.Volunteer;

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
        public async Task<IActionResult> CreateVolunteerHours(CreateVolunteerHoursDto dto)
        {
            var result = await _volunteerRequestService.CreateVolunteerHoursAsync(dto);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }

        [HttpGet("request/{requestId}")]
        public async Task<IActionResult> GetHoursByRequestId(int requestId)
        {
            var result = await _volunteerRequestService.GetHoursByRequestIdAsync(requestId);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }
            return Ok(result.Value);
        }

        [HttpGet("request/{requestId}/date-range")]
        public async Task<IActionResult> GetHoursByDateRange(int requestId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var result = await _volunteerRequestService.GetHoursByDateRangeAsync(requestId, startDate, endDate);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }
            return Ok(result.Value);
        }

        [HttpPut("{hoursId}")]
        public async Task<IActionResult> UpdateVolunteerHours(int hoursId, CreateVolunteerHoursDto dto)
        {
            var result = await _volunteerRequestService.UpdateVolunteerHoursAsync(hoursId, dto);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }

        [HttpDelete("{hoursId}")]
        public async Task<IActionResult> DeleteVolunteerHours(int hoursId)
        {
            var result = await _volunteerRequestService.DeleteVolunteerHoursAsync(hoursId);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }

        // ===== APROBACIÓN DE HORAS =====
        [HttpPost("{hoursId}/approve")]
        [Authorize(Roles = "AdminSistema")] // ← CAMBIAR A AdminSistema para ser consistente
        public async Task<IActionResult> ApproveHours(int hoursId, [FromBody] ApproveHoursRequestDto dto)
        {
            var approveDto = new ApproveRejectHoursDto
            {
                HoursId = hoursId,
                IsApproved = true,
                ApproverId = dto.ApproverId,
                ApproverName = dto.ApproverName
            };

            var result = await _volunteerRequestService.ApproveHoursAsync(approveDto);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }

        [HttpPost("{hoursId}/reject")]
        [Authorize(Roles = "AdminSistema")] // ← CAMBIAR A AdminSistema para ser consistente
        public async Task<IActionResult> RejectHours(int hoursId, [FromBody] RejectHoursRequestDto dto)
        {
            var rejectDto = new ApproveRejectHoursDto
            {
                HoursId = hoursId,
                IsApproved = false,
                RejectionReason = dto.RejectionReason,
                ApproverId = dto.ApproverId,
                ApproverName = dto.ApproverName
            };

            var result = await _volunteerRequestService.RejectHoursAsync(rejectDto);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }

        [HttpGet("pending")]
        [Authorize(Roles = "AdminSistema")] // ← CAMBIAR A AdminSistema para ser consistente
        public async Task<IActionResult> GetPendingHours()
        {
            var hours = await _volunteerRequestService.GetPendingHoursAsync();
            return Ok(hours);
        }

        // ===== VALIDACIONES =====
        [HttpPost("validate")]
        public async Task<IActionResult> ValidateHours(CreateVolunteerHoursDto dto)
        {
            var result = await _volunteerRequestService.ValidateHoursAsync(dto);
            return Ok(new { isValid = result.IsSuccess, errors = result.Errors });
        }
    }
}
using Api.Abstractions.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.Volunteer;
using Shared.Enums;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VolunteerRequestController : ControllerBase
    {
        private readonly IVolunteerRequestService _volunteerRequestService;

        public VolunteerRequestController(IVolunteerRequestService volunteerRequestService)
        {
            _volunteerRequestService = volunteerRequestService;
        }

        // ===== MÉTODOS EXISTENTES =====
        [HttpGet("Volunteer/{volunteerId}")]
        public async Task<IActionResult> GetVolunteerRequests(int volunteerId)
        {
            var requests = await _volunteerRequestService.GetAllByVolunteerIDAsync(volunteerId);
            return Ok(requests);
        }

        [HttpPost]
        public async Task<IActionResult> CreateVolunteerRequest(VolunteerRequestDto volunteerRequestDto)
        {
            var result = await _volunteerRequestService.CreateAsync(volunteerRequestDto);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }

        // ===== NUEVOS ENDPOINTS PARA ADMINISTRACIÓN =====
        [HttpGet]
        public async Task<IActionResult> GetAllRequests()
        {
            var requests = await _volunteerRequestService.GetAllRequestsAsync();
            return Ok(requests);
        }

        [HttpGet("state/{state}")]
        public async Task<IActionResult> GetRequestsByState(VolunteerState state)
        {
            var requests = await _volunteerRequestService.GetRequestsByStateAsync(state);
            return Ok(requests);
        }

        [HttpGet("{requestId}")]
        public async Task<IActionResult> GetRequestById(int requestId)
        {
            var result = await _volunteerRequestService.GetRequestByIdAsync(requestId);
            if (result.IsFailure)
            {
                return NotFound(result.Errors);
            }
            return Ok(result.Value);
        }

        [HttpPost("{requestId}/approve")]
        public async Task<IActionResult> ApproveRequest(int requestId, [FromBody] int approverId)
        {
            var result = await _volunteerRequestService.ApproveRequestAsync(requestId, approverId);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }

        [HttpPost("{requestId}/reject")]
        public async Task<IActionResult> RejectRequest(int requestId, [FromBody] RejectRequestDto dto)
        {
            var result = await _volunteerRequestService.RejectRequestAsync(requestId, dto.ApproverId, dto.Reason);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }


    }
}
  


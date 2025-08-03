using Api.Abstractions.Application;
using Microsoft.AspNetCore.Authorization;
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

        // ===== VOLUNTARIO =====
        // Obtener todas las solicitudes de un voluntario
        [HttpGet("Volunteer/{volunteerId}")]
        public async Task<IActionResult> GetVolunteerRequests(int volunteerId)
        {
            var requests = await _volunteerRequestService.GetAllByVolunteerIDAsync(volunteerId);
            return Ok(requests);
        }

        // Crear nueva solicitud (voluntario)
        [HttpPost]
        public async Task<IActionResult> CreateVolunteerRequest([FromBody] VolunteerRequestDto volunteerRequestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _volunteerRequestService.CreateAsync(volunteerRequestDto);
            if (result.IsFailure)
                return BadRequest(new { errors = result.Errors });

            return Ok();
        }

        // Obtener solicitud por id (puede usarla voluntario o admin)
        [HttpGet("{requestId}")]
        public async Task<IActionResult> GetRequestById(int requestId)
        {
            var result = await _volunteerRequestService.GetRequestByIdAsync(requestId);
            if (result.IsFailure)
                return NotFound(new { errors = result.Errors });

            return Ok(result.Value);
        }

        // ===== ADMINISTRACIÓN =====
        // Todas las solicitudes
        [HttpGet]
        [Authorize(Roles = "AdminSistema")]
        public async Task<IActionResult> GetAllRequests()
        {
            var requests = await _volunteerRequestService.GetAllRequestsAsync();
            return Ok(requests);
        }

        // Filtrar por estado
        [HttpGet("state/{state}")]
        [Authorize(Roles = "AdminSistema")]
        public async Task<IActionResult> GetRequestsByState(VolunteerState state)
        {
            var requests = await _volunteerRequestService.GetRequestsByStateAsync(state);
            return Ok(requests);
        }

        // Aprobar solicitud
        [HttpPost("{requestId}/approve")]
        [Authorize(Roles = "AdminSistema")]
        public async Task<IActionResult> ApproveRequest(int requestId, [FromBody] ApproveRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _volunteerRequestService.ApproveRequestAsync(requestId, dto.ApproverId);
            if (result.IsFailure)
                return BadRequest(new { errors = result.Errors });

            return Ok();
        }

        // Rechazar solicitud
        [HttpPost("{requestId}/reject")]
        [Authorize(Roles = "AdminSistema")]
        public async Task<IActionResult> RejectRequest(int requestId, [FromBody] RejectRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _volunteerRequestService.RejectRequestAsync(requestId, dto.ApproverId, dto.Reason);
            if (result.IsFailure)
                return BadRequest(new { errors = result.Errors });

            return Ok();
        }
    }
}


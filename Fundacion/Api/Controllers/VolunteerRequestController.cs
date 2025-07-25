using Api.Abstractions.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.Volunteer;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VolunteerRequestController : ControllerBase
    {
        private readonly IVolunteerRequestService volunteerRequestService;
        public VolunteerRequestController(IVolunteerRequestService volunteerRequestService)
        {
            this.volunteerRequestService = volunteerRequestService;
        }
        [HttpGet("Volunteer/{volunteerId}")]
        public async Task<IActionResult> GetVolunteerRequests(int volunteerId)
        {
            var requests = await volunteerRequestService.GetAllByVolunteerIDAsync(volunteerId);
            return Ok(requests);
        }
        [HttpPost]
        public async Task<IActionResult> CreateVolunteerRequest(VolunteerRequestDto volunteerRequestDto)
        {
            var result = await volunteerRequestService.CreateAsync(volunteerRequestDto);
            if (result.IsFailure) { 
                return BadRequest(result.Errors);
            }
            return Ok();
        }

    }
}

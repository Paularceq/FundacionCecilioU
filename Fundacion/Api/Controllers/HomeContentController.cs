using Api.Abstractions.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.PublicContent;
using System.Security.Claims;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HomeContentController : ControllerBase
    {
        private readonly IHomeContentService _homeContentService;

        public HomeContentController(IHomeContentService homeContentService)
        {
            _homeContentService = homeContentService;
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllHomeContent()
        {
            var result = await _homeContentService.GetAllHomeContentAsync();
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }

            // ✅ CAMBIO: Devolver datos directamente
            return Ok(result.Value);
        }

        [HttpGet("Active")]
        [AllowAnonymous] // Mantener público
        public async Task<IActionResult> GetActiveHomeContent()
        {
            var result = await _homeContentService.GetActiveHomeContentAsync();
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }

            // ✅ CAMBIO: Devolver datos directamente
            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetHomeContentById(int id)
        {
            var result = await _homeContentService.GetHomeContentByIdAsync(id);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }

            // ✅ CAMBIO: Devolver datos directamente
            return Ok(result.Value);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateHomeContent(CreateHomeContentDto contentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _homeContentService.CreateHomeContentAsync(contentDto, userId);

            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }

            return NoContent(); // ✅ Consistente
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateHomeContent(UpdateHomeContentDto contentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _homeContentService.UpdateHomeContentAsync(contentDto);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }

            return NoContent(); // ✅ Consistente
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHomeContent(int id)
        {
            var result = await _homeContentService.DeleteHomeContentAsync(id);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }

            return NoContent(); // ✅ Consistente
        }

        [HttpPost("ToggleStatus/{id}")]
        public async Task<IActionResult> ToggleHomeContentStatus(int id)
        {
            var result = await _homeContentService.ToggleHomeContentStatusAsync(id);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }

            return NoContent(); // ✅ Consistente
        }
    }
}
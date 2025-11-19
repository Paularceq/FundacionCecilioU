using Api.Abstractions.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class NewsletterController : ControllerBase
    {
        private readonly INewsletterService _newsletterService;

        public NewsletterController(INewsletterService newsletterService)
        {
            _newsletterService = newsletterService;
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllNewsletters()
        {
            var result = await _newsletterService.GetAllNewslettersAsync();
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }

            // ✅ CAMBIO: Devolver datos directamente
            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNewsletterById(int id)
        {
            var result = await _newsletterService.GetNewsletterByIdAsync(id);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }

            // ✅ CAMBIO: Devolver datos directamente
            return Ok(result.Value);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateNewsletter(CreateNewsletterDto newsletterDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _newsletterService.CreateNewsletterAsync(newsletterDto, userId);

            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }

            return NoContent(); // ✅ Consistente con otros endpoints de creación
        }

        [HttpPost("Send/{id}")]
        public async Task<IActionResult> SendNewsletter(int id)
        {
            var result = await _newsletterService.SendNewsletterAsync(id);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }

            return NoContent(); // ✅ Consistente
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNewsletter(int id)
        {
            var result = await _newsletterService.DeleteNewsletterAsync(id);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }

            return NoContent(); // ✅ Consistente
        }

        [HttpPost("Preview")]
        public async Task<IActionResult> GenerateNewsletterPreview(CreateNewsletterDto newsletterDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _newsletterService.GenerateNewsletterPreviewAsync(newsletterDto);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }

            // ✅ CAMBIO: Devolver HTML directamente
            return Ok(result.Value);
        }
    }
}
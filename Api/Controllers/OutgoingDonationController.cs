using Api.Abstractions.Application;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.OutgoingDonations;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OutgoingDonationController : ControllerBase
    {
        private readonly IOutgoingDonationService _outgoingDonationService;

        public OutgoingDonationController(IOutgoingDonationService outgoingDonationService)
        {
            _outgoingDonationService = outgoingDonationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var donations = await _outgoingDonationService.GetAllAsync();
            return Ok(donations);
        }

        [HttpGet("requester/{requesterId:int}")]
        public async Task<IActionResult> GetAllByRequesterIdAsync(int requesterId)
        {
            var donations = await _outgoingDonationService.GetAllByRequesterIdAsync(requesterId);
            return Ok(donations);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var donation = await _outgoingDonationService.GetByIdAsync(id);
            if (donation == null)
            {
                return NotFound();
            }
            return Ok(donation);
        }

        [HttpPost("in-kind-donation")]
        public async Task<IActionResult> CreateInKindDonationAsync(CreateInKindDonationDto donationDto)
        {
            var result = await _outgoingDonationService.CreateInKindDonationsAsync(donationDto);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }

            return Ok();
        }

        [HttpPost("in-kind-donation/resolve")]
        public async Task<IActionResult> ResolveInKindDonationAsync(ResolveOutgoingDonationRequestDto resolveDto)
        {
            var result = await _outgoingDonationService.ResolveInKindDonationAsync(resolveDto);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }

            return Ok();
        }
    }
}

using Api.Abstractions.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.Donations;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonationController : ControllerBase
    {
        private readonly IDonationService _donationService;

        public DonationController(IDonationService donationService)
        {
            _donationService = donationService;

        }

        [HttpPost("add-MonetaryDonation")]
        public  async Task<IActionResult> AddMonetaryDonationAsync(AddMonetaryDonationDto monetaryDonationDto)
        {
            var result = await _donationService.AddMonetaryDonationAsync(monetaryDonationDto);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }

        //crear endpoint para consultar las donaciones en el donation service 
    }
    
}

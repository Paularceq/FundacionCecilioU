using Api.Abstractions.Application;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos;

namespace Api.Controllers
{
    [Route("api/UserManagement")]
    [ApiController]
    public class UserManagementController : ControllerBase

    {
        private readonly IUserManagementService _userManagementService;

        public UserManagementController(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        [HttpGet("AllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userManagementService.GetAllUsersAsync();
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }
            return Ok(result.Value);

        }
        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUser(NewUserDto userDto)
        {
            var result = await _userManagementService.AddUserAsync(userDto);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }
            return NoContent();
        }
        [HttpGet("AllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var result = await _userManagementService.GetAllRoles();
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }
            return Ok(result.Value);
        }
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser(UpdateUserDto userDto)
        {
            var result = await _userManagementService.UpdateUserAsync(userDto);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }
            return NoContent();
        }
        [HttpGet("UserById/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _userManagementService.GetUserByIdAsync(id);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }
            return Ok(result.Value);
        }
        [HttpPost("ChangeUserStatus/{id}")]
        public async Task<IActionResult> ChangeUserStatus(int id)
        {
            var result = await _userManagementService.ChangeUserStatus(id);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }
            return NoContent();
        }

        [HttpGet("UsersByRole/{roleName}")]
        public async Task<IActionResult> GetUsersByRole(string roleName)
        {
            var result = await _userManagementService.GetUsersByRole(roleName);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }
            return Ok(result.Value);
        }
    }

}

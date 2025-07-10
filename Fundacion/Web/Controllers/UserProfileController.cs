using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Web.Extensions;
using Web.Services;

namespace Web.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly UserProfileService _userProfileService;
        public UserProfileController(UserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }
        public async Task<IActionResult> Index()
        {
            int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var userProfile = await _userProfileService.GetUserProfileAsync(userId);
            if (userProfile.IsFailure)
            {
                this.SetErrorMessage(userProfile.Errors);
                return RedirectToAction("Index", "Home");
            }
            return View(userProfile.Value);
        }
        [HttpGet]
        public async Task<IActionResult> Update()
        {
            int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var userProfile = await _userProfileService.GetUserProfileAsync(userId);
            if (userProfile.IsFailure)
            {
                this.SetErrorMessage(userProfile.Errors);
                return RedirectToAction("Update", "Home");
            }
            var viewModel = new UserProfileUpdateDto
            {
                Id = userProfile.Value.Id,
                Nombre = userProfile.Value.Nombre,
                Apellidos = userProfile.Value.Apellidos,
                Email = userProfile.Value.Email,
                Nacionalidad = userProfile.Value.Nacionalidad,
                Identificacion = userProfile.Value.Identificacion
            };
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Update(UserProfileUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return View(updateDto);
            }
            int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            updateDto.Id = userId;
            var result = await _userProfileService.UpdateUserProfileAsync(updateDto);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                return View(updateDto);
            }
            this.SetSuccessMessage("Perfil actualizado correctamente.");
            return RedirectToAction("Index");
        }
    }
}

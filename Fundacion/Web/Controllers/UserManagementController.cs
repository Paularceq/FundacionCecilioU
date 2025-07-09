using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.Extensions;
using Web.Models.UserManagement;
using Web.Services;

namespace Web.Controllers
{
    [Authorize(Roles = "AdminUsuarios")]
    public class UserManagementController : Controller
    {
        private readonly UserManagementService _userManagementService;

        public UserManagementController(UserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        
        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            var users = await _userManagementService.GetAllUsersAsync();
            return View(users.Value);
        }

        
        [HttpGet]
        public async Task<IActionResult> AddUserAsync()
        {
            var roles = await _userManagementService.GetAllRolesAsync();
            var model = new AddUserViewModel
            {
                Roles = roles.Value.Select(role=>new SelectListItem
                {
                    Value = role.Name,
                    Text = role.Description,
                }).ToList()
            };


            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> AddUserAsync(AddUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var roles = await _userManagementService.GetAllRolesAsync();
                model.Roles = roles.Value.Select(role => new SelectListItem
                {
                    Value = role.Name,
                    Text = role.Description,
                }).ToList();
                return View(model);
            }
            var result = await _userManagementService.AddUserAsync(model);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                var roles = await _userManagementService.GetAllRolesAsync();
                model.Roles = roles.Value.Select(role => new SelectListItem
                {
                    Value = role.Name,
                    Text = role.Description,
                }).ToList();
                return View(model);
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> UpdateUserAsync(int id)
        {
            var user = await _userManagementService.GetUserByIdAsync(id);
            if (user.IsFailure)
            {
                this.SetErrorMessage(user.Errors);
                return RedirectToAction("Index");
            }
            var roles = await _userManagementService.GetAllRolesAsync();
            var model = new UpdateUserViewModel
            {
                Id = user.Value.Id,
                Nombre = user.Value.Nombre,
                Apellidos = user.Value.Apellidos,
                Email = user.Value.Email,
                Nacionalidad = user.Value.Nacionalidad,
                Identificacion = user.Value.Identificacion,
                SelectedRoles = user.Value.Roles.Select(r => r.Name).ToList(),
                Roles = roles.Value.Select(role => new SelectListItem
                {
                    Value = role.Name,
                    Text = role.Description,
                    Selected = user.Value.Roles.Any(r => r.Name == role.Name)
                }).ToList()
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateUserAsync(UpdateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var roles = await _userManagementService.GetAllRolesAsync();
                model.Roles = roles.Value.Select(role => new SelectListItem
                {
                    Value = role.Name,
                    Text = role.Description,
                }).ToList();
                return View(model);
            }
            var result = await _userManagementService.UpdateUserAsync(model);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                var roles = await _userManagementService.GetAllRolesAsync();
                model.Roles = roles.Value.Select(role => new SelectListItem
                {
                    Value = role.Name,
                    Text = role.Description,
                }).ToList();
                return View(model);
            }
            this.SetSuccessMessage("Usuario actualizado correctamente.");
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> ChangeUserStatusAsync(int id)
        {
            var user = await _userManagementService.GetUserByIdAsync(id);
            if (user.IsFailure)
            {
                this.SetErrorMessage(user.Errors);
                return RedirectToAction("Index");
            }
            var result = await _userManagementService.ChangeUserStatusAsync(id);
            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
                return RedirectToAction("Index");
            }
            this.SetSuccessMessage($"El usuario {user.Value.NombreCompleto} ha sido {(user.Value.Activo ? "activado" : "desactivado")} correctamente.");
            return RedirectToAction("Index");
        }


    }


}



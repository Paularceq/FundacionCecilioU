using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.Extensions;
using Web.Models.UserManagement;
using Web.Services;

namespace Web.Controllers
{
    public class UserManagementController : Controller
    {
        private readonly UserManagementService _userManagementService;

        public UserManagementController(UserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            var users = await _userManagementService.GetAllUsersAsync();
            return View(users.Value);
        }

        [Authorize]
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
        [Authorize]
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


    }


}



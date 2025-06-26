using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        //    [Authorize]
        //    [HttpGet]
        //    //public async Task<IActionResult> AddUserAsync()
        //    {
        //        //var roles = // obtener roles del api (RoleController->RoleService->RoleRepository)
        //        //var model = new AddUserViewModel { Roles = roles };

        //        // return View(model);
        //    }

        //    //[HttpPost]
        //    //public async Task<IActionResult> AddUserAsync(AddUserViewModel model)...
        //
    }
}



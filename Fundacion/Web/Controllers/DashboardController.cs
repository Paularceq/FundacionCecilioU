using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Constants;

namespace Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = Roles.AdminSistema)]  
        public IActionResult ProtectedForAdmins()
        {
            return View();
        }
    }
}

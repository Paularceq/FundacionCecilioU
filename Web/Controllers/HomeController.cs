using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        // Inyección de dependencia para el logger (registro de eventos)
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Acción para la vista principal (Index.cshtml)
        public IActionResult Index()
        {
            return View();
        }

        // Acción para la vista de privacidad (Privacy.cshtml)
        public IActionResult Privacy()
        {
            return View();
        }

        // Acción para la vista de contacto (Contacto.cshtml)
        public IActionResult Contacto()
        {
            return View();
        }

        // Acción para la vista de donaciones (Donaciones.cshtml)
        public IActionResult Donaciones()
        {
            return View();
        }

        // Acción para la vista "Quiénes Somos" (QuienesSomos.cshtml)
        public IActionResult QuienesSomos()
        {
            return View();
        }

        // Acción para manejar errores y mostrar la vista Error.cshtml
        [HttpGet]
        public IActionResult Error(string? message)
        {
            ViewData["ErrorMessage"] = message ?? "La sesión ha expirado o se ha iniciado en otro dispositivo.";
            return View();
        }
    }
}



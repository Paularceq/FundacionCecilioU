using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        // Inyecci�n de dependencia para el logger (registro de eventos)
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Acci�n para la vista principal (Index.cshtml)
        public IActionResult Index()
        {
            return View();
        }

        // Acci�n para la vista de privacidad (Privacy.cshtml)
        public IActionResult Privacy()
        {
            return View();
        }

        // Acci�n para la vista de contacto (Contacto.cshtml)
        public IActionResult Contacto()
        {
            return View();
        }

        // Acci�n para la vista de donaciones (Donaciones.cshtml)
        public IActionResult Donaciones()
        {
            return View();
        }

        // Acci�n para la vista "Qui�nes Somos" (QuienesSomos.cshtml)
        public IActionResult QuienesSomos()
        {
            return View();
        }

        // Acci�n para manejar errores y mostrar la vista Error.cshtml
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Se crea un modelo de error con el ID de la solicitud actual
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}



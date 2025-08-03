using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.Becas;
using System.Net.Http.Headers;
using Web.Models.Becas;

namespace Web.Controllers
{
    public class SolicitudBecaController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _env;

        public SolicitudBecaController(IHttpClientFactory httpClientFactory, IWebHostEnvironment env)
        {
            _httpClientFactory = httpClientFactory;
            _env = env;
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(SolicitudBecaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Aquí podrías subir los archivos a disco o a una API si tenés configurado
            var cartaConsentBytes = await LeerArchivoComoBytes(model.CartaConsentimiento);
            var cartaNotasBytes = await LeerArchivoComoBytes(model.CartaNotas);

            var cliente = _httpClientFactory.CreateClient("API");

            var data = new
            {
                cedulaEstudiante = model.CedulaEstudiante,
                nombreEstudiante = model.NombreEstudiante,
                correoContacto = model.CorreoContacto,
                telefonoContacto = model.TelefonoContacto,
                direccion = model.Direccion,
                colegio = model.Colegio,
                nivelEducativo = model.NivelEducativo,
                cartaConsentimiento = cartaConsentBytes,
                cartaConsentimientoContentType = model.CartaConsentimiento?.ContentType,
                cartaNotas = cartaNotasBytes,
                cartaNotasContentType = model.CartaNotas?.ContentType,
                esFormularioManual = false
            };

            var response = await cliente.PostAsJsonAsync("SolicitudesBeca", data);
            var errorMsg = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return RedirectToAction("VerBecas");
            //ModelState.AddModelError("", "Ocurrió un error al enviar la solicitud.");
            ModelState.AddModelError("", $"Ocurrió un error al enviar la solicitud: ");
            return View(model);
        }
        private async Task<byte[]> LeerArchivoComoBytes(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
                return null;
            using var ms = new MemoryStream();
            await archivo.CopyToAsync(ms);
            return ms.ToArray();
        }
        [HttpGet]
        public async Task<IActionResult> VerBecas()
        {
            var cliente = _httpClientFactory.CreateClient("API");
            var response = await cliente.GetAsync("SolicitudesBeca");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "No se pudieron cargar las solicitudes.";
                return View(new List<SolicitudBecaViewModel>());
            }

            var solicitudesDto = await response.Content.ReadFromJsonAsync<List<SolicitudBecaDto>>();

            var solicitudesViewModel = solicitudesDto.Select(dto => new SolicitudBecaViewModel
            {
               Id = dto.Id,
                CedulaEstudiante = dto.CedulaEstudiante,
                NombreEstudiante = dto.NombreEstudiante,
                CorreoContacto = dto.CorreoContacto,
                TelefonoContacto = dto.TelefonoContacto,
                Direccion = dto.Direccion,
                Colegio = dto.Colegio,
                NivelEducativo = dto.NivelEducativo,
                Estado = dto.Estado,
                CartaConsentimientoBytes = dto.CartaConsentimiento,
                CartaConsentimientoContentType = dto.CartaConsentimientoContentType,
                CartaNotasBytes = dto.CartaNotas,
                CartaNotasContentType = dto.CartaNotasContentType
            }).ToList();

            return View(solicitudesViewModel);
        }
        //[HttpGet]
        //public IActionResult VerBecas()
        //{
        //    // Aquí podrías obtener las solicitudes desde tu API o base de datos
        //    // Por ejemplo, si usás una API:
        //    var cliente = _httpClientFactory.CreateClient("API");
        //    var response = cliente.GetAsync("SolicitudesBeca").Result;

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        ViewBag.Error = "No se pudieron cargar las solicitudes.";
        //        return View(new List<SolicitudBecaViewModel>());
        //    }

        //    var solicitudes = response.Content.ReadFromJsonAsync<List<SolicitudBecaViewModel>>().Result;
        //    return View(solicitudes);
        //}

        //private async Task<string> GuardarArchivo(IFormFile archivo, string subcarpeta)


    }
}

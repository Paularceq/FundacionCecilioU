using Microsoft.AspNetCore.Mvc;
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
                return RedirectToAction("Gracias");

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

        //private async Task<string> GuardarArchivo(IFormFile archivo, string subcarpeta)
        //{
        //    if (archivo == null || archivo.Length == 0)
        //        return null;

        //    var uploads = Path.Combine(_env.WebRootPath, "uploads", subcarpeta);
        //    Directory.CreateDirectory(uploads);

        //    var fileName = $"{Guid.NewGuid()}_{archivo.FileName}";
        //    var filePath = Path.Combine(uploads, fileName);

        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await archivo.CopyToAsync(stream);
        //    }

        //    // Ruta relativa para guardar en la base
        //    return $"/uploads/{subcarpeta}/{fileName}";
        //}

        public IActionResult Gracias()
        {
            return View();
        }
    }
}

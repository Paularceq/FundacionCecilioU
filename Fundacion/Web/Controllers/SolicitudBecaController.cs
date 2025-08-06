using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.Becas;
using System.Net;
using Web.Extensions;
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

            var dto = new Shared.Dtos.Becas.SolicitudBecaDto
            {
                CedulaEstudiante = model.CedulaEstudiante,
                NombreEstudiante = model.NombreEstudiante,
                CorreoContacto = model.CorreoContacto,
                TelefonoContacto = model.TelefonoContacto,
                Direccion = model.Direccion,
                Colegio = model.Colegio,
                NivelEducativo = model.NivelEducativo,
                CartaConsentimiento = cartaConsentBytes,
                CartaConsentimientoContentType = model.CartaConsentimiento?.ContentType,
                CartaNotas = cartaNotasBytes,
                CartaNotasContentType = model.CartaNotas?.ContentType,
                EsFormularioManual = false
            };

            var response = await cliente.PostAsJsonAsync("SolicitudesBeca", dto);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    ModelState.AddModelError("CedulaEstudiante", "Ya existe una solicitud con esta cédula.");
                }
                else
                {
                    this.SetErrorMessage("Ocurrió un error al enviar la solicitud.");
                }

                return View(model);
            }
            this.SetSuccessMessage("Solicitud enviada correctamente.");
            return RedirectToAction("VerBecas");

            //var errorMsg = await response.Content.ReadAsStringAsync();

            //if (response.IsSuccessStatusCode)
            //    return RedirectToAction("VerBecas");
            ////ModelState.AddModelError("", "Ocurrió un error al enviar la solicitud.");
            //ModelState.AddModelError("", $"Ocurrió un error al enviar la solicitud: ");
            //return View(model);
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

        // GET: /SolicitudesBeca/TomarDecision/12345678
        public async Task<IActionResult> TomarDecision(int Id)
        {
            var cliente = _httpClientFactory.CreateClient("API");


            var response = await cliente.GetFromJsonAsync<SolicitudBecaDto>($"SolicitudesBeca/{Id}");
            if (response == null)
            {
                TempData["Error"] = "No se encontró la solicitud.";
                return RedirectToAction("Index");
            }

            var model = new SolicitudBecaViewModel
            {
                CedulaEstudiante = response.CedulaEstudiante,
                NombreEstudiante = response.NombreEstudiante,
                CorreoContacto = response.CorreoContacto,
                TelefonoContacto = response.TelefonoContacto,
                Direccion = response.Direccion,
                Colegio = response.Colegio,
                NivelEducativo = response.NivelEducativo,
                Estado = response.Estado,
                //MontoAsignado = response.MontoAsignado,
                CartaConsentimientoBytes = response.CartaConsentimiento,
                CartaConsentimientoContentType = response.CartaConsentimientoContentType,
                CartaNotasBytes = response.CartaNotas,
                CartaNotasContentType = response.CartaNotasContentType
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarDecision(SolicitudBecaViewModel model)
        {

            var cliente = _httpClientFactory.CreateClient("API");

            var dto = new TomarDesicionDto
            {

                Estado = model.Estado
            };

            var response = await cliente.PutAsJsonAsync($"solicitudesbeca/decidir/{model.Id}", dto);

            if (response.IsSuccessStatusCode)
            {
                this.SetSuccessMessage("La decisión se guardó correctamente.");
                return RedirectToAction("VerBecas");
            }
            var consultaSolicitud = await cliente.GetFromJsonAsync<SolicitudBecaDto>($"SolicitudesBeca/{model.Id}");
            if (consultaSolicitud == null)
            {
                TempData["Error"] = "No se encontró la solicitud.";
                return RedirectToAction("Index");
            }

            model.CedulaEstudiante = consultaSolicitud.CedulaEstudiante;
            model.NombreEstudiante = consultaSolicitud.NombreEstudiante;
            model.CorreoContacto = consultaSolicitud.CorreoContacto;
            model.TelefonoContacto = consultaSolicitud.TelefonoContacto;
            model.Direccion = consultaSolicitud.Direccion;
            model.Colegio = consultaSolicitud.Colegio;
            model.NivelEducativo = consultaSolicitud.NivelEducativo;
            model.Estado = consultaSolicitud.Estado;
            model.CartaConsentimientoBytes = consultaSolicitud.CartaConsentimiento;
            model.CartaConsentimientoContentType = consultaSolicitud.CartaConsentimientoContentType;
            model.CartaNotasBytes = consultaSolicitud.CartaNotas;
            model.CartaNotasContentType = consultaSolicitud.CartaNotasContentType;


            this.SetErrorMessage("Ocurrió un error al guardar la decisión.");
            return View("TomarDecision", model);
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

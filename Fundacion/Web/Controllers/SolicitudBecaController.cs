using Microsoft.AspNetCore.Mvc;
using Shared.Constants;
using Shared.Dtos.Becas;
using System.Net;
using System.Security.Claims;
using Web.Extensions;
using Web.Helpers.Attributes;
using Web.Models.Becas;

namespace Web.Controllers
{
    public class SolicitudBecaController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SolicitudBecaController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Crear()
        {
            string? nombreEstudiante = null;
            string? cedulaEstudiante = null;

            if (User.IsInRole(Roles.Estudiante))
            {
                nombreEstudiante = User.FindFirst(ClaimTypes.Name)?.Value;
                cedulaEstudiante = User.FindFirst(ClaimTypes.PrimarySid)?.Value;
            }

            var model = new SolicitudBecaViewModel { NombreEstudiante = nombreEstudiante, CedulaEstudiante = cedulaEstudiante };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(SolicitudBecaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var cartaConsentBytes = await LeerArchivoComoBytes(model.CartaConsentimiento);
            var cartaNotasBytes = await LeerArchivoComoBytes(model.CartaNotas);

            var cliente = _httpClientFactory.CreateClient("API");

            int? idEstudiante = null;

            if (User.IsInRole(Roles.Estudiante))
            {
                idEstudiante = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            }

            var dto = new SolicitudBecaDto
            {
                EstudianteId = idEstudiante,
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
        }

        private static async Task<byte[]> LeerArchivoComoBytes(IFormFile archivo)
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
            if (User.IsInRole(Roles.AdminBecas) || User.IsInRole(Roles.AdminSistema))
            {
                return await ListarParaAdmin();
            }

            return await ListarParaEstudiante();
        }

        private async Task<IActionResult> ListarParaAdmin()
        {
            var cliente = _httpClientFactory.CreateClient("API");
            var response = await cliente.GetAsync("SolicitudesBeca");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "No se pudieron cargar las solicitudes.";
                return View(nameof(VerBecas), new List<SolicitudBecaViewModel>());
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
            })
                .OrderByDescending(s => s.Id)
                .ToList();

            return View(nameof(VerBecas), solicitudesViewModel);
        }

        private async Task<IActionResult> ListarParaEstudiante()
        {
            var idEstudiante = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var cliente = _httpClientFactory.CreateClient("API");
            var response = await cliente.GetAsync($"SolicitudesBeca/estudiante/{idEstudiante}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "No se pudieron cargar las solicitudes.";
                return View(nameof(VerBecas), new List<SolicitudBecaViewModel>());
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
            })
                .OrderByDescending(s => s.Id)
                .ToList();

            return View(nameof(VerBecas), solicitudesViewModel);
        }

        [AuthorizeRoles(Roles.AdminSistema, Roles.AdminBecas)]
        public async Task<IActionResult> TomarDecision(int Id)
        {
            var cliente = _httpClientFactory.CreateClient("API");

            var response = await cliente.GetFromJsonAsync<SolicitudBecaDto>($"SolicitudesBeca/{Id}");
            if (response == null)
            {
                TempData["Error"] = "No se encontró la solicitud.";
                return RedirectToAction("Index");
            }

            var solicitud = new SolicitudBecaViewModel
            {
                CedulaEstudiante = response.CedulaEstudiante,
                NombreEstudiante = response.NombreEstudiante,
                CorreoContacto = response.CorreoContacto,
                TelefonoContacto = response.TelefonoContacto,
                Direccion = response.Direccion,
                Colegio = response.Colegio,
                NivelEducativo = response.NivelEducativo,
                Estado = response.Estado,
                CartaConsentimientoBytes = response.CartaConsentimiento,
                CartaConsentimientoContentType = response.CartaConsentimientoContentType,
                CartaNotasBytes = response.CartaNotas,
                CartaNotasContentType = response.CartaNotasContentType
            };

            var model = new TomarDesicionViewModel
            {
                Solicitud = solicitud,
                Id = response.Id,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarDecision(TomarDesicionViewModel model)
        {
            var cliente = _httpClientFactory.CreateClient("API");

            if (!ModelState.IsValid)
            {
                var solicitud = await cliente.GetFromJsonAsync<SolicitudBecaDto>($"SolicitudesBeca/{model.Id}");
                if (solicitud == null)
                {
                    TempData["Error"] = "No se encontró la solicitud.";
                    return RedirectToAction("Index");
                }

                var solicitudModel = new SolicitudBecaViewModel
                {
                    CedulaEstudiante = solicitud.CedulaEstudiante,
                    NombreEstudiante = solicitud.NombreEstudiante,
                    CorreoContacto = solicitud.CorreoContacto,
                    TelefonoContacto = solicitud.TelefonoContacto,
                    Direccion = solicitud.Direccion,
                    Colegio = solicitud.Colegio,
                    NivelEducativo = solicitud.NivelEducativo,
                    Estado = solicitud.Estado,
                    CartaConsentimientoBytes = solicitud.CartaConsentimiento,
                    CartaConsentimientoContentType = solicitud.CartaConsentimientoContentType,
                    CartaNotasBytes = solicitud.CartaNotas,
                    CartaNotasContentType = solicitud.CartaNotasContentType
                };

                model.Solicitud = solicitudModel;

                this.SetErrorMessage("Ocurrió un error al guardar la decisión.");
                return View("TomarDecision", model);
            }

            var dto = new TomarDesicionDto
            {

                Estado = model.Estado,
                Amount = model.Amount,
                Currency = model.Currency,
                Frequency = model.Frequency,
                StartDate = model.StartDate,
                EndDate = model.EndDate
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

            var solicitudViewModel = new SolicitudBecaViewModel
            {
                CedulaEstudiante = consultaSolicitud.CedulaEstudiante,
                NombreEstudiante = consultaSolicitud.NombreEstudiante,
                CorreoContacto = consultaSolicitud.CorreoContacto,
                TelefonoContacto = consultaSolicitud.TelefonoContacto,
                Direccion = consultaSolicitud.Direccion,
                Colegio = consultaSolicitud.Colegio,
                NivelEducativo = consultaSolicitud.NivelEducativo,
                Estado = consultaSolicitud.Estado,
                CartaConsentimientoBytes = consultaSolicitud.CartaConsentimiento,
                CartaConsentimientoContentType = consultaSolicitud.CartaConsentimientoContentType,
                CartaNotasBytes = consultaSolicitud.CartaNotas,
                CartaNotasContentType = consultaSolicitud.CartaNotasContentType
            };

            model.Solicitud = solicitudViewModel;

            this.SetErrorMessage("Ocurrió un error al guardar la decisión.");
            return View("TomarDecision", model);
        }

        [HttpGet]
        public async Task<IActionResult> DetallesBeca(int id)
        {
            var cliente = _httpClientFactory.CreateClient("API");

            try
            {
                var response = await cliente.GetAsync($"SolicitudesBeca/detalles-beca/{id}");

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return View(null); // Devuelve null a la vista si la API responde con 404
                }

                response.EnsureSuccessStatusCode();

                var detallesBeca = await response.Content.ReadFromJsonAsync<ScholarshipDetailsDto>();
                return View(detallesBeca);
            }
            catch (Exception ex)
            {
                // Manejo de errores generales
                TempData["Error"] = "Ocurrió un error al obtener los detalles de la beca.";
                return View(null);
            }
        }
    }
}

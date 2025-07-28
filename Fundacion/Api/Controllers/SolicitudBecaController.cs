using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.Becas;
using Microsoft.EntityFrameworkCore;
using Api.Database;
using Api.Database.Entities;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolicitudesBecaController : ControllerBase

    {
        private readonly HttpClient _httpClient;

        public SolicitudesBecaController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        private readonly DatabaseContext _context;

        public SolicitudesBecaController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/SolicitudesBeca
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var solicitudes = await _context.SolicitudesBeca.ToListAsync();
            return Ok(solicitudes);
        }

        // GET: api/SolicitudesBeca/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var solicitud = await _context.SolicitudesBeca.FindAsync(id);
            if (solicitud == null)
                return NotFound();

            return Ok(solicitud);
        }

        // POST: api/SolicitudesBeca
        [HttpPost]
        public async Task<IActionResult> CrearSolicitud([FromBody] SolicitudBecaDto dto)
        {
            if (!ModelState.IsValid)
            return BadRequest(ModelState);

            var solicitud = new SolicitudBeca
            {
                CedulaEstudiante = dto.CedulaEstudiante,
                NombreEstudiante = dto.NombreEstudiante,
                CorreoContacto = dto.CorreoContacto,
                TelefonoContacto = dto.TelefonoContacto,
                Direccion = dto.Direccion,
                Colegio = dto.Colegio,
                NivelEducativo = dto.NivelEducativo,
                CartaConsentimiento = dto.CartaConsentimiento,
                CartaNotas = dto.CartaNotas,
                FechaSolicitud = DateTime.UtcNow,
                Estado = EstadoSolicitud.Pendiente,
                EsFormularioManual = dto.EsFormularioManual
            };
            _context.SolicitudesBeca.Add(solicitud);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = solicitud.Id }, solicitud);
        }

        // PUT: api/SolicitudesBeca/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] SolicitudBeca solicitud)
        {
            if (id != solicitud.Id)
                return BadRequest("El ID no coincide.");

            if (!await _context.SolicitudesBeca.AnyAsync(x => x.Id == id))
                return NotFound();

            _context.Entry(solicitud).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/SolicitudesBeca/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var solicitud = await _context.SolicitudesBeca.FindAsync(id);
            if (solicitud == null)
                return NotFound();

            _context.SolicitudesBeca.Remove(solicitud);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

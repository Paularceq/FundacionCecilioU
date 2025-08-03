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
        private readonly DatabaseContext _context;


        public SolicitudesBecaController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/SolicitudesBeca
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var solicitudes = await _context.SolicitudesBeca
    .Select(s => new SolicitudBecaDto
    {
        CedulaEstudiante = s.CedulaEstudiante,
        NombreEstudiante = s.NombreEstudiante,
        CorreoContacto = s.CorreoContacto,
        TelefonoContacto = s.TelefonoContacto,
        Direccion = s.Direccion,
        Colegio = s.Colegio,
        NivelEducativo = s.NivelEducativo,
        CartaConsentimiento = s.CartaConsentimiento,
        CartaConsentimientoContentType = s.CartaConsentimientoContentType,
        CartaNotas = s.CartaNotas,
        CartaNotasContentType = s.CartaNotasContentType,
        FechaSolicitud = s.FechaSolicitud,
        Estado = s.Estado.ToString(),
        EsFormularioManual = s.EsFormularioManual
    })
    .ToListAsync();

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
        [HttpGet("cedula/{cedula}")]
        public async Task<ActionResult<SolicitudBecaDto>> GetByCedula(string cedula)
        {
            var s = await _context.SolicitudesBeca
                .Where(x => x.CedulaEstudiante == cedula)
                .Select(x => new SolicitudBecaDto
                {
                    CedulaEstudiante = x.CedulaEstudiante,
                    NombreEstudiante = x.NombreEstudiante,
                    CorreoContacto = x.CorreoContacto,
                    TelefonoContacto = x.TelefonoContacto,
                    Direccion = x.Direccion,
                    Colegio = x.Colegio,
                    NivelEducativo = x.NivelEducativo,
                    CartaConsentimiento = x.CartaConsentimiento,
                    CartaConsentimientoContentType = x.CartaConsentimientoContentType,
                    CartaNotas = x.CartaNotas,
                    CartaNotasContentType = x.CartaNotasContentType,
                    FechaSolicitud = x.FechaSolicitud,
                    Estado = x.Estado.ToString(),
                    EsFormularioManual = x.EsFormularioManual
                })
                .FirstOrDefaultAsync();

            if (s == null) return NotFound();
            return Ok(s);
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
                CartaConsentimientoContentType = dto.CartaConsentimientoContentType,
               CartaNotas = dto.CartaNotas,
               CartaNotasContentType = dto.CartaNotasContentType,
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

        [HttpPut("decidir/{cedula}")]
        public async Task<IActionResult> TomarDecision(
     string cedula,
     [FromBody] SolicitudBecaDto dto)
        {
            var solicitud = await _context.SolicitudesBeca
                .FirstOrDefaultAsync(s => s.CedulaEstudiante == cedula);

            if (solicitud == null)
                return NotFound();

            
            if (!Enum.TryParse<EstadoSolicitud>(
                    dto.Estado,
                    ignoreCase: true,
                    out var nuevoEstado))
            {
                ModelState.AddModelError(
                    nameof(dto.Estado),
                    $"Valor inválido para Estado: {dto.Estado}"
                );
                return BadRequest(ModelState);
            }

            solicitud.Estado = nuevoEstado;
            solicitud.MontoAsignado = dto.MontoAsignado;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/SolicitudesBeca/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Eliminar(int id)
        //{
        //    var solicitud = await _context.SolicitudesBeca.FindAsync(id);
        //    if (solicitud == null)
        //        return NotFound();

        //    _context.SolicitudesBeca.Remove(solicitud);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

    }
}

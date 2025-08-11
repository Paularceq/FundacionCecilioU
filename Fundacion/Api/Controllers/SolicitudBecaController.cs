using Api.Abstractions.Application;
using Api.Database;
using Api.Database.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Dtos.Becas;
using Shared.Enums;

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
                    Id = s.Id,
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
            var dto = new SolicitudBecaDto
            {
                Id = solicitud.Id,
                CedulaEstudiante = solicitud.CedulaEstudiante,
                NombreEstudiante = solicitud.NombreEstudiante,
                CorreoContacto = solicitud.CorreoContacto,
                TelefonoContacto = solicitud.TelefonoContacto,
                Direccion = solicitud.Direccion,
                Colegio = solicitud.Colegio,
                NivelEducativo = solicitud.NivelEducativo,
                CartaConsentimiento = solicitud.CartaConsentimiento,
                CartaConsentimientoContentType = solicitud.CartaConsentimientoContentType,
                CartaNotas = solicitud.CartaNotas,
                CartaNotasContentType = solicitud.CartaNotasContentType,
                FechaSolicitud = solicitud.FechaSolicitud,
                Estado = solicitud.Estado.ToString(),
                EsFormularioManual = solicitud.EsFormularioManual
            };
            return Ok(dto);
        }
        //[HttpGet("cedula/{cedula}")]
        //public async Task<ActionResult<SolicitudBecaDto>> GetByCedula(string cedula)
        //{
        //    var s = await _context.SolicitudesBeca
        //        .Where(x => x.CedulaEstudiante == cedula)
        //        .Select(x => new SolicitudBecaDto
        //        {
        //            CedulaEstudiante = x.CedulaEstudiante,
        //            NombreEstudiante = x.NombreEstudiante,
        //            CorreoContacto = x.CorreoContacto,
        //            TelefonoContacto = x.TelefonoContacto,
        //            Direccion = x.Direccion,
        //            Colegio = x.Colegio,
        //            NivelEducativo = x.NivelEducativo,
        //            CartaConsentimiento = x.CartaConsentimiento,
        //            CartaConsentimientoContentType = x.CartaConsentimientoContentType,
        //            CartaNotas = x.CartaNotas,
        //            CartaNotasContentType = x.CartaNotasContentType,
        //            FechaSolicitud = x.FechaSolicitud,
        //            Estado = x.Estado.ToString(),
        //            EsFormularioManual = x.EsFormularioManual
        //        })
        //        .FirstOrDefaultAsync();

        //    if (s == null) return NotFound();
        //    return Ok(s);
        //}


        // POST: api/SolicitudesBeca

        [HttpPost]
        public async Task<IActionResult> CrearSolicitud([FromBody] SolicitudBecaDto dto)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var solicitudExistente = await _context.SolicitudesBeca
        .AnyAsync(s => s.CedulaEstudiante == dto.CedulaEstudiante);

            if (solicitudExistente)
            {
                return Conflict(new
                {
                    mensaje = "Ya existe una solicitud de beca con esta cédula."
                });
            }


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
            try
            {
                _context.SolicitudesBeca.Add(solicitud);
                await _context.SaveChangesAsync();

                // Si todo va bien, devolvemos 201 con la entidad creada
                return CreatedAtAction(nameof(GetById), new { id = solicitud.Id }, solicitud);
            }
            catch (DbUpdateException dbEx)
            {
                // Captura errores de actualización de EF Core (e.g. violaciones de FK, tamaño de columna, not null)
                var errorMsg = dbEx.InnerException?.Message ?? dbEx.Message;
                return StatusCode(500, new { error = errorMsg });
            }
            catch (Exception ex)
            {
                // Para cualquier otro tipo de excepción
                return StatusCode(500, new { error = ex.Message });
            }

            //_context.SolicitudesBeca.Add(solicitud);
            //await _context.SaveChangesAsync();

            // return CreatedAtAction(nameof(GetById), new { id = solicitud.Id }, solicitud);
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

        [HttpPut("decidir/{id}")]
        public async Task<IActionResult> TomarDecision(int id, TomarDesicionDto dto)
        {
            var solicitud = await _context.SolicitudesBeca
                .FirstOrDefaultAsync(s => s.Id == id);

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

            await _context.SaveChangesAsync();

            if (nuevoEstado == EstadoSolicitud.Aprobada)
            {
                var scholarship = new Scholarship
                {
                    RequestId = solicitud.Id,
                    Amount = dto.Amount ?? 0,
                    Currency = dto.Currency ?? Currency.CRC, 
                    Frequency = dto.Frequency ?? ScholarshipFrequency.Monthly,
                    StartDate = dto.StartDate ?? DateTime.Now,
                    EndDate = dto.EndDate,
                    IsActive = true
                };

                _context.Set<Scholarship>().Add(scholarship);
                await _context.SaveChangesAsync();
            }

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

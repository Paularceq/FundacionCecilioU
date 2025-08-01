using System;
using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos
{
    public class SolicitudBecaDto
    {
        [Required]
        [StringLength(20)]
        public string CedulaEstudiante { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreEstudiante { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string CorreoContacto { get; set; }

        [Required]
        [StringLength(20)]
        public string TelefonoContacto { get; set; }

        [Required]
        [StringLength(250)]
        public string Direccion { get; set; }

        [Required]
        [StringLength(100)]
        public string Colegio { get; set; }

        [Required]
        [StringLength(50)]
        public string NivelEducativo { get; set; }

        // Estos campos van a contener los archivos en base64 o alguna referencia si se usa IFormFile en API
        [Required]
        public string CartaConsentimientoBase64 { get; set; }

        [Required]
        public string CartaNotasBase64 { get; set; }

        public bool EsFormularioManual { get; set; } = false;
    }
}

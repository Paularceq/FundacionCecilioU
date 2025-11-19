using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.Becas
{
    public class SolicitudBecaDto
    {
        public int Id { get; set; }

        public int? EstudianteId { get; set; }

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

        [Required(ErrorMessage = "La Carta de consentimiento es obligatoria")]
        public byte[] CartaConsentimiento { get; set; }
        public string CartaConsentimientoContentType { get; set; }

        [Required(ErrorMessage = "La carta de notas es obligatoria")]
        public byte[] CartaNotas { get; set; }
        public string CartaNotasContentType { get; set; }

        public DateTime FechaSolicitud { get; set; } = DateTime.Now;

        public string Estado { get; set; } = "Pendiente";

        public bool EsFormularioManual { get; set; } = false;
    }
}

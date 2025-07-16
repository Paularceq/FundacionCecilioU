using System.ComponentModel.DataAnnotations;

namespace Web.Models.Becas
{
    public class SolicitudBecaViewModel
    {
        [Required]
        [StringLength(20)]
        public string CedulaEstudiante { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreEstudiante { get; set; }

        [Required]
        [EmailAddress]
        public string CorreoContacto { get; set; }

        [StringLength(20)]
        public string TelefonoContacto { get; set; }

        [StringLength(250)]
        public string Direccion { get; set; }

        [Required]
        [StringLength(100)]
        public string Colegio { get; set; }

        [Required]
        [StringLength(50)]
        public string NivelEducativo { get; set; }

        public IFormFile CartaConsentimiento { get; set; }

        public IFormFile CartaNotas { get; set; }
    }
}

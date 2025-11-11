using System.ComponentModel.DataAnnotations;

namespace Web.Models.Becas
{
    public class SolicitudBecaViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Se requiere la cédula del estudiante.")]
        [StringLength(20, ErrorMessage = "La cédula no puede tener más de 20 caracteres.")]
        [Display(Name = "Cédula del estudiante")]
        [RegularExpression(@"^\d-\d{4}-\d{4}$", ErrorMessage = "El formato de la cédula debe ser 0-0000-0000.")]
        public string CedulaEstudiante { get; set; }

        [Required(ErrorMessage = "Se requiere el nombre del estudiante.")]
        [StringLength(100, ErrorMessage = "El nombre del estudiante no puede tener más de 100 caracteres.")]
        [Display(Name = "Nombre del estudiante")]
        public string NombreEstudiante { get; set; }

        [Required(ErrorMessage = "Se requiere el correo del encargado.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
        [Display(Name = "Correo del encargado")]
        public string CorreoContacto { get; set; }

        [StringLength(20, ErrorMessage = "El teléfono no puede tener más de 20 caracteres.")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "El formato del teléfono debe ser 8 dígitos (99999999).")]
        [Display(Name = "Teléfono de contacto")]
        public string TelefonoContacto { get; set; }

        [StringLength(250, ErrorMessage = "La dirección no puede tener más de 250 caracteres.")]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "Se requiere el nombre de la institución educativa del estudiante.")]
        [StringLength(100, ErrorMessage = "El nombre de la institución educativa no puede tener más de 100 caracteres.")]
        [Display(Name = "Institución educativa")]
        public string Colegio { get; set; }

        [Required(ErrorMessage = "Se requiere el nivel educativo del estudiante.")]
        [StringLength(50, ErrorMessage = "El nivel educativo no puede tener más de 50 caracteres.")]
        [Display(Name = "Nivel educativo")]
        public string NivelEducativo { get; set; }

        [Display(Name = "Carta de consentimiento")]
        [Required(ErrorMessage = "Debe adjuntar la carta de consentimiento.")]
        public IFormFile CartaConsentimiento { get; set; }

        [Display(Name = "Carta de notas")]
        [Required(ErrorMessage = "Debe adjuntar la carta de notas.")]
        public IFormFile CartaNotas { get; set; }

        public byte[] CartaConsentimientoBytes { get; set; }
        public string CartaConsentimientoContentType { get; set; }

        public byte[] CartaNotasBytes { get; set; }
        public string CartaNotasContentType { get; set; }

        [Display(Name = "Comentario del administrador")]
        public string ComentarioAdministrador { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Pendiente";
    }
}

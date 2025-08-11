using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.Enums;
using Shared.Extensions;
using System.ComponentModel.DataAnnotations;
using Web.Helpers;

namespace Web.Models.Becas
{
    public class SolicitudBecaViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Se requiere la Cedula del estudiante")]
        [StringLength(20)]
        [Display(Name = "Cedula del estudiante")]
        public string CedulaEstudiante { get; set; }

        [Required(ErrorMessage = "Se requiere el nombre del estudiante")]
        [StringLength(100)]
        [Display(Name = "Nombre del Estudiante")]
        public string NombreEstudiante { get; set; }


        [Required(ErrorMessage = "Se requiere el correo del encargado")]
        [EmailAddress]
        [Display(Name = "Correo del engargado")]
        public string CorreoContacto { get; set; }

        [StringLength(20)]
        public string TelefonoContacto { get; set; }

        [StringLength(250)]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "Se requiere el nombre del colegio del Estudiante")]
        [StringLength(100)]
        public string Colegio { get; set; }

        [Required(ErrorMessage = "Se requiere el niveleducativo del Estudiante")]
        [StringLength(50)]
        [Display(Name = "Nivel Educativo")]
        public string NivelEducativo { get; set; }

        public IFormFile CartaConsentimiento { get; set; }

        public IFormFile CartaNotas { get; set; }

        public byte[] CartaConsentimientoBytes { get; set; }
        public string CartaConsentimientoContentType { get; set; }

        public byte[] CartaNotasBytes { get; set; }
        public string CartaNotasContentType { get; set; }

        [Display(Name = "Comentario")]
        public string ComentarioAdministrador { get; set; }

        public string Estado { get; set; } = "Pendiente";

        public decimal? MontoAsignado { get; set; }
        public decimal? Amount { get; set; }
        public Currency? Currency { get; set; }
        public ScholarshipFrequency? Frequency { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public IEnumerable<SelectListItem> CurrencyList { get; set; } = EnumHelper.ToSelectListItems<Currency>();
        public IEnumerable<SelectListItem> FrequencyList { get; set; } = EnumHelper.ToSelectListItems<ScholarshipFrequency>();
    }
}

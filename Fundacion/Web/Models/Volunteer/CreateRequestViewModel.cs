using System.ComponentModel.DataAnnotations;
namespace Web.Models.Volunteer
{
    public class CreateRequestViewModel
    {
        [Required(ErrorMessage = "La institución es obligatoria")]
        [StringLength(200, ErrorMessage = "La institución no puede exceder 200 caracteres")]
        [Display(Name = "Institución")]
        public string Institution { get; set; } = string.Empty;

        [Required(ErrorMessage = "La profesión es obligatoria")]
        [StringLength(100, ErrorMessage = "La profesión no puede exceder 100 caracteres")]
        [Display(Name = "Profesión")]
        public string Profession { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        [Display(Name = "Descripción del trabajo")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Las horas son obligatorias")]
        [Range(1, 200, ErrorMessage = "Las horas deben estar entre 1 y 200")]
        [Display(Name = "Horas propuestas")]
        public int Hours { get; set; }
    }
}
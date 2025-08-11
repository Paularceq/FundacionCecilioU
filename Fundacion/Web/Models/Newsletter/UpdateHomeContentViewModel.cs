using System.ComponentModel.DataAnnotations;

namespace Web.Models.Newsletter
{
    public class UpdateHomeContentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [Display(Name = "Título")]
        [StringLength(200, ErrorMessage = "El título no puede exceder los 200 caracteres.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [Display(Name = "Descripción")]
        [StringLength(1000, ErrorMessage = "La descripción no puede exceder los 1000 caracteres.")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "URL de Imagen")]
        [StringLength(500, ErrorMessage = "La URL de imagen no puede exceder los 500 caracteres.")]
        public string ImageUrl { get; set; } = string.Empty;

        [Display(Name = "Está Activo")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Fecha de Inicio")]
        [DataType(DataType.DateTime)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Fecha de Fin")]
        [DataType(DataType.DateTime)]
        public DateTime? EndDate { get; set; }
    }
}
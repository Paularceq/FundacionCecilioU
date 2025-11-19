using System.ComponentModel.DataAnnotations;

namespace Web.Models.Newsletter
{
    public class CreateNewsletterViewModel
    {
        [Required(ErrorMessage = "El asunto es obligatorio.")]
        [Display(Name = "Asunto")]
        [StringLength(300, ErrorMessage = "El asunto no puede exceder los 300 caracteres.")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "El contenido personalizado es obligatorio.")]
        [Display(Name = "Contenido Personalizado")]
        public string CustomContent { get; set; } = string.Empty;

        [Display(Name = "Fecha de Envío")]
        [DataType(DataType.DateTime)]
        public DateTime? SendDate { get; set; }

        [Display(Name = "Incluir contenido del home")]
        public bool IncludeHomeContent { get; set; } = true;

        [Display(Name = "Enviar ahora")]
        public bool SendNow { get; set; } = false;
    }
}
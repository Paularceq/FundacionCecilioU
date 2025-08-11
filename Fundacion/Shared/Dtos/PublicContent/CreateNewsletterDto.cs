using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.PublicContent
{
    public class CreateNewsletterDto
    {
        [Required(ErrorMessage = "El asunto es obligatorio.")]
        [StringLength(300, ErrorMessage = "El asunto no puede exceder los 300 caracteres.")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "El contenido personalizado es obligatorio.")]
        public string CustomContent { get; set; } = string.Empty;

        public DateTime? SendDate { get; set; }
        public bool IncludeHomeContent { get; set; } = true;
        public bool SendNow { get; set; } = false;
    }
}

using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
        [StringLength(200, ErrorMessage = "El email no puede exceder los 200 caracteres.")]
        public string Email { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Web.Models.Auth
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
        [Display(Name = "Correo electrónico", Description = "Correo electrónico del usuario.")]
        public string Email { get; set; }
    }
}

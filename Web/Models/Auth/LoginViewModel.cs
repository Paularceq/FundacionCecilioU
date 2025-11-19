using System.ComponentModel.DataAnnotations;

namespace Web.Models.Auth
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
        [Display(Name = "Correo electrónico", Description = "Correo electrónico del usuario.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña", Description = "Contraseña para la cuenta.")]
        public string Password { get; set; }

        [Display(Name = "Recordarme", Description = "Mantener la sesión iniciada.")]
        public bool RememberMe { get; set; }
    }
}

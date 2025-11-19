using System.ComponentModel.DataAnnotations;

namespace Web.Models.Auth
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "El token es obligatorio.")]
        public string Token { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña", Description = "Contraseña para la cuenta.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirmar la contraseña es obligatorio.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar contraseña", Description = "Confirme la contraseña ingresada.")]
        public string ConfirmPassword { get; set; }
    }
}

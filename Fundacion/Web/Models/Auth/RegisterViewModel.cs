using System.ComponentModel.DataAnnotations;

namespace Web.Models.Auth
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        [Display(Name = "Nombre", Description = "Nombre del usuario.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [StringLength(150, ErrorMessage = "Los apellidos no pueden exceder los 150 caracteres.")]
        [Display(Name = "Apellidos", Description = "Apellidos del usuario.")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
        [StringLength(200, ErrorMessage = "El email no puede exceder los 200 caracteres.")]
        [Display(Name = "Correo electrónico", Description = "Correo electrónico del usuario.")]
        public string Email { get; set; }

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

        [Required(ErrorMessage = "La nacionalidad es obligatoria.")]
        [Display(Name = "Nacionalidad", Description = "Nacionalidad del usuario.")]
        public string Nacionalidad { get; set; }

        [Required(ErrorMessage = "La identificación es obligatoria.")]
        [StringLength(50, ErrorMessage = "La identificación no puede exceder los 50 caracteres.")]
        [Display(Name = "Identificación", Description = "Número de identificación del usuario.")]
        public string Identificacion { get; set; }
    }
}

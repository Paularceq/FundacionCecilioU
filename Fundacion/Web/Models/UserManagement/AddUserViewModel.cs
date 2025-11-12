using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.Dtos;
using System.ComponentModel.DataAnnotations;
using Web.Helpers.Validation;

namespace Web.Models.UserManagement
{
    public class AddUserViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [StringLength(150, ErrorMessage = "Los apellidos no pueden exceder los 150 caracteres.")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
        [StringLength(200, ErrorMessage = "El email no puede exceder los 200 caracteres.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La identificación es obligatoria.")]
        [RegularExpression(@"^\d-\d{4}-\d{4}$", ErrorMessage = "La identificación debe tener el formato 9-9999-9999.")]
        public string Identificacion { get; set; }

        public List<SelectListItem> Roles { get; set; } = new List<SelectListItem>();
        [MinCount(1, ErrorMessage = "Debe seleccionar al menos un rol.")] 
        public List<string> SelectedRoles { get; set; } = new List<string>();

    }
}
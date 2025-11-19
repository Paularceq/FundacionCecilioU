using System.ComponentModel.DataAnnotations;
using Shared.Enums;

namespace Web.Models.Newsletter
{
    public class SubscribeViewModel
    {
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [Display(Name = "Nombre Completo")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Frecuencia de Boletín")]
        public SubscriptionFrequency Frequency { get; set; } = SubscriptionFrequency.Weekly;
    }
}

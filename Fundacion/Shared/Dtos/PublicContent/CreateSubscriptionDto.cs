using System.ComponentModel.DataAnnotations;
using Shared.Enums;

namespace Shared.Dtos.PublicContent
{
    public class CreateSubscriptionDto
    {
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
        [StringLength(200, ErrorMessage = "El email no puede exceder los 200 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Name { get; set; } = string.Empty;

        public SubscriptionFrequency Frequency { get; set; } = SubscriptionFrequency.Weekly;
    }
}
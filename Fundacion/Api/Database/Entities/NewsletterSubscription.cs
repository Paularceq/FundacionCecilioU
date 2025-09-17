using System.ComponentModel.DataAnnotations;
using Shared.Enums;

namespace Api.Database.Entities
{
    public class NewsletterSubscription
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
        [StringLength(200, ErrorMessage = "El email no puede exceder los 200 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public DateTime SubscriptionDate { get; set; } = DateTime.UtcNow;

        [StringLength(100, ErrorMessage = "El token de confirmación no puede exceder los 100 caracteres.")]
        public string ConfirmationToken { get; set; } = string.Empty;

        public SubscriptionFrequency Frequency { get; set; } = SubscriptionFrequency.Weekly;
    }
}
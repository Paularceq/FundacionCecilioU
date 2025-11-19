using System.ComponentModel.DataAnnotations;
using Shared.Enums;

namespace Api.Database.Entities
{
    public class Newsletter
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(300, ErrorMessage = "El asunto no puede exceder los 300 caracteres.")]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string CustomContent { get; set; } = string.Empty;

        public DateTime? SendDate { get; set; }
        public NewsletterStatus Status { get; set; } = NewsletterStatus.Draft;
        public int RecipientCount { get; set; } = 0;

        [Required]
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation property
        public User Creator { get; set; }
    }
}
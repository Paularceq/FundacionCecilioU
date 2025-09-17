using System.ComponentModel.DataAnnotations;

namespace Api.Database.Entities
{
    public class HomeContent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "El título no puede exceder los 200 caracteres.")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000, ErrorMessage = "La descripción no puede exceder los 1000 caracteres.")]
        public string Description { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La URL de imagen no puede exceder los 500 caracteres.")]
        public string ImageUrl { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [Required]
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation property
        public User Creator { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace Api.Database.Entities
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "El nombre del rol no puede exceder los 50 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "La descripción del rol no puede exceder los 200 caracteres.")]
        public string Description { get; set; } = string.Empty;

        public ICollection<User> Users { get; set; } = [];
    }
}

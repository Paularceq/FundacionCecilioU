using System.ComponentModel.DataAnnotations;

namespace Api.Database.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(100)]
        public string Apellidos { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; }

        [Required]
        [StringLength(20)]
        public string Nacionalidad { get; set; }

        [Required]
        [StringLength(20)]
        public string Identificacion { get; set; }

        [Required]
        public string PasswordHash { get; set; }
        
        public bool RequiereCambioDePassword { get; set; }

        public string NombreCompleto => $"{Nombre} {Apellidos}";

        public ICollection<Role> Roles { get; set; } = [];
    }
}

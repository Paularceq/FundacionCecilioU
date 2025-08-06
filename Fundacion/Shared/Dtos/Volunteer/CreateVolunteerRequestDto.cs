
using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.Volunteer
{
    public class CreateVolunteerRequestDto
    {
        [Required(ErrorMessage = "La institución es obligatoria")]
        public string Institution { get; set; } = string.Empty;

        [Required(ErrorMessage = "La profesión es obligatoria")]
        public string Profession { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Las horas propuestas son obligatorias")]
        [Range(1, 200, ErrorMessage = "Las horas deben estar entre 1 y 200")]
        public int Hours { get; set; }
    }
}
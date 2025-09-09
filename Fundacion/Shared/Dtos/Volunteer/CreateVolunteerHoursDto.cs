using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.Volunteer
{
    public class CreateVolunteerHoursDto
    {
        [Required]
        public int VolunteerRequestId { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "La hora de inicio es obligatoria")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "La hora de fin es obligatoria")]
        public TimeSpan EndTime { get; set; }

        [Required(ErrorMessage = "Debe describir las actividades realizadas")]
        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        public string ActivitiesDescription { get; set; } = string.Empty;

        public string? Notes { get; set; }

    }
}

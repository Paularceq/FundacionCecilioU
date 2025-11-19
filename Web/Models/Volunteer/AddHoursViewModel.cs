using System.ComponentModel.DataAnnotations;

namespace Web.Models.Volunteer
{
    public class AddHoursViewModel
    {
        public int? Id { get; set; } // Para edición

        [Required]
        public int RequestId { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [Display(Name = "Fecha de trabajo")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "La hora de inicio es obligatoria")]
        [Display(Name = "Hora de inicio")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; } = TimeSpan.FromHours(8); // 8:00 AM

        [Required(ErrorMessage = "La hora de fin es obligatoria")]
        [Display(Name = "Hora de fin")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; } = TimeSpan.FromHours(17); // 5:00 PM

        [Required(ErrorMessage = "Debe describir las actividades realizadas")]
        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        [Display(Name = "Actividades realizadas")]
        [DataType(DataType.MultilineText)]
        public string ActivitiesDescription { get; set; } = string.Empty;

        [Display(Name = "Notas adicionales")]
        [StringLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres")]
        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; }

        // Propiedades calculadas para la vista
        public decimal TotalHours
        {
            get
            {
                if (EndTime > StartTime)
                {
                    var difference = EndTime - StartTime;
                    return (decimal)difference.TotalHours;
                }
                return 0;
            }
        }

        public string TotalHoursFormatted => $"{TotalHours:N1} horas";
        public bool IsValid => StartTime < EndTime && TotalHours <= 8 && TotalHours >= 1;
        public bool IsEditing => Id.HasValue;
    }
}
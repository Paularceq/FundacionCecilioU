using System.ComponentModel.DataAnnotations;
namespace Web.Models.Volunteer
{
    public class CreateVolunteerHoursViewModel
    {
        [Required]
        public int VolunteerRequestId { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [Display(Name = "Fecha de trabajo")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "La hora de inicio es obligatoria")]
        [Display(Name = "Hora de inicio")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; } = new TimeSpan(8, 0, 0);

        [Required(ErrorMessage = "La hora de fin es obligatoria")]
        [Display(Name = "Hora de fin")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; } = new TimeSpan(17, 0, 0);

        [Required(ErrorMessage = "Debe describir las actividades realizadas")]
        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        [Display(Name = "Actividades realizadas")]
        [DataType(DataType.MultilineText)]
        public string ActivitiesDescription { get; set; } = string.Empty;

        [Display(Name = "Notas adicionales")]
        [StringLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres")]
        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; }

        // ✅ PROPIEDADES ADICIONALES PARA VALIDACIÓN
        [Display(Name = "Horas a registrar")]
        public decimal CalculatedHours => (decimal)(EndTime - StartTime).TotalHours;

        // Información contextual para validaciones
        public decimal TotalHoursRequested { get; set; }
        public decimal TotalHoursApproved { get; set; }
        public decimal RemainingHours => TotalHoursRequested - TotalHoursApproved;

        // ✅ REQUERIMIENTO 1: Validación de horas restantes
        public bool IsValidHours => CalculatedHours <= RemainingHours && CalculatedHours > 0;

        // ✅ Para mostrar información en la vista
        public string ValidationMessage
        {
            get
            {
                if (CalculatedHours <= 0)
                    return "La hora de fin debe ser mayor a la hora de inicio.";

                if (CalculatedHours > 8)
                    return "No se pueden registrar más de 8 horas por día.";

                if (CalculatedHours > RemainingHours)
                    return $"No puedes registrar {CalculatedHours:F1} horas. Solo quedan {RemainingHours:F1} horas disponibles.";

                return string.Empty;
            }
        }

        // ✅ REQUERIMIENTO 3: Información sobre registros existentes para la fecha
        public bool HasExistingHoursForDate { get; set; }
        public string? ExistingHoursState { get; set; }
        public string? ExistingHoursMessage { get; set; }
    }
}
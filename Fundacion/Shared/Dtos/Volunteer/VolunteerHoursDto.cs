using Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.Volunteer
{
    public class VolunteerHoursDto
    {
        public int Id { get; set; }
        [Required]
        public int VolunteerRequestId { get; set; }
        public int? ApproverId { get; set; }                
        public string VolunteerName { get; set; } = string.Empty;        

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [Display(Name = "Fecha de trabajo")]
        public DateTime Date { get; set; }
        [Required(ErrorMessage = "La hora de inicio es obligatoria")]
        [Display(Name = "Hora de inicio")]
        public TimeSpan StartTime { get; set; }
        [Required(ErrorMessage = "La hora de fin es obligatoria")]
        [Display(Name = "Hora de fin")]
        public TimeSpan EndTime { get; set; }
        [Display(Name = "Total de horas")]
        public decimal TotalHours { get; set; }
        [Required(ErrorMessage = "Debe describir las actividades realizadas")]
        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        [Display(Name = "Actividades realizadas")]
        public string ActivitiesDescription { get; set; } = string.Empty;
        [Display(Name = "Notas adicionales")]
        public string? Notes { get; set; }
        public VolunteerState State { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? RejectionReason { get; set; }
        public string RegisteredBy { get; set; } = string.Empty;

        // Propiedades calculadas
        public string StateDescription => State switch
        {
            VolunteerState.Pending => "Pendiente",
            VolunteerState.Approved => "Aprobado",
            VolunteerState.Rejected => "Rechazado",
            _ => "Desconocido"
        };
    }
}
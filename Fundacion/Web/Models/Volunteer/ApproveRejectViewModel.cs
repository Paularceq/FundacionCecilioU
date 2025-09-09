using System.ComponentModel.DataAnnotations;
namespace Web.Models.Volunteer
{
    public class ApproveRejectViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Type { get; set; } = string.Empty; // "request" o "hours"

        [Required]
        public bool IsApproved { get; set; }

        [Display(Name = "Razón del rechazo")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "La razón debe tener entre 10 y 500 caracteres")]
        public string? RejectionReason { get; set; }

        public int ApproverId { get; set; }
        public string ApproverName { get; set; } = string.Empty;

        // Para mostrar información del item a aprobar/rechazar
        public string ItemDescription { get; set; } = string.Empty;
        public string VolunteerName { get; set; } = string.Empty;
        public DateTime ItemDate { get; set; }

        // Validación simple en el controlador en lugar de atributo personalizado
        public bool IsValid()
        {
            if (!IsApproved && string.IsNullOrWhiteSpace(RejectionReason))
                return false;
            if (!IsApproved && RejectionReason?.Length < 10)
                return false;
            return true;
        }
    }
}
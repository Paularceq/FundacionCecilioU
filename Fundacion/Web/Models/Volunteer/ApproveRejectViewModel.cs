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

        [RequiredIf("IsApproved", false, ErrorMessage = "La razón del rechazo es obligatoria")]
        [Display(Name = "Razón del rechazo")]
        [StringLength(500, ErrorMessage = "La razón no puede exceder 500 caracteres")]
        public string? RejectionReason { get; set; }

        public int ApproverId { get; set; }
        public string ApproverName { get; set; } = string.Empty;

        // Para mostrar información del item a aprobar/rechazar
        public string ItemDescription { get; set; } = string.Empty;
        public string VolunteerName { get; set; } = string.Empty;
        public DateTime ItemDate { get; set; }
    }

    // Atributo de validación personalizado
    public class RequiredIfAttribute : ValidationAttribute
    {
        private readonly string _propertyName;
        private readonly object _value;

        public RequiredIfAttribute(string propertyName, object value)
        {
            _propertyName = propertyName;
            _value = value;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(_propertyName);
            if (property == null)
                return new ValidationResult($"Property {_propertyName} not found");

            var propertyValue = property.GetValue(validationContext.ObjectInstance);

            if (Equals(propertyValue, _value) && (value == null || string.IsNullOrWhiteSpace(value.ToString())))
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
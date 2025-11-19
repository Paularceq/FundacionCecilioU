using Shared.Enums;
using System.ComponentModel.DataAnnotations;
using Web.Helpers.Validation;

namespace Web.Models.Financial
{
    public class LeaseIncomeViewModel
    {
        [Required(ErrorMessage = "El monto es obligatorio.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "La moneda es obligatoria.")]
        public Currency Currency { get; set; }

        [Required(ErrorMessage = "El nombre del inquilino es obligatorio.")]
        public string TenantName { get; set; }

        [Required(ErrorMessage = "La identificación del inquilino es obligatoria.")]
        public string TenantIdentification { get; set; }

        [Required(ErrorMessage = "El uso es obligatorio.")]
        public string Usage { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
        [DateGreaterOrEqualThan("StartDate", "La fecha de fin debe ser mayor o igual a la fecha de inicio.")]
        public DateTime EndDate { get; set; }

        public IFormFile ReceiptFile { get; set; }
    }
}

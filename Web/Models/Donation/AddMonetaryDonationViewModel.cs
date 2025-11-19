using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.Enums;

namespace Web.Models.Donation
{
    public class AddMonetaryDonationViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [Display(Name = "Nombre completo")]
        public string Name { get; set; }

        [Required(ErrorMessage = "La identificación es obligatoria.")]
        [Display(Name = "Número de identificación")]
        [RegularExpression(@"^\d-\d{4}-\d{4}$", ErrorMessage = "El formato de la identificación debe ser 0-0000-0000.")]
        public string Identification { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor que cero.")]
        [Display(Name = "Monto a donar")]
        public double Amount { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una moneda.")]
        [Display(Name = "Moneda")]
        public Currency SelectedCurrency { get; set; }

        public List<SelectListItem> AvailableCurrencies { get; set; } = [];
    }
}

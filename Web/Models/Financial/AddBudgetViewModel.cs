using System.ComponentModel.DataAnnotations;
using Web.Helpers.Validation;
using Shared.Enums;

namespace Web.Models.Financial;

public class AddBudgetViewModel
{
    [Required(ErrorMessage = "El monto es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor que 0.")]
    [Display(Name = "Monto")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Debe seleccionar una moneda.")]
    [Display(Name = "Moneda")]
    public Currency Currency { get; set; }

    [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
    [MinYearsAgo(10, "La fecha de inicio no puede ser anterior a 10 años atrás.")]
    [Display(Name = "Fecha de inicio")]
    public DateTime StartDate { get; set; } = new DateTime(DateTime.Now.Year, 1, 1);

    [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
    [DateGreaterThan(nameof(StartDate), "La fecha de fin debe ser mayor que la fecha de inicio.")]
    [Display(Name = "Fecha de fin")]
    public DateTime EndDate { get; set; } = new DateTime(DateTime.Now.Year, 1, 1);
}

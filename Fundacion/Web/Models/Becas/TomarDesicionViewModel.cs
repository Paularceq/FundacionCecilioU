using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.Enums;
using System.ComponentModel.DataAnnotations;
using Web.Helpers;
using Web.Helpers.Validation;

namespace Web.Models.Becas;

public class TomarDesicionViewModel
{
    public SolicitudBecaViewModel Solicitud { get; set; }

    public int Id { get; set; }

    [Display(Name = "Comentario del administrador")]
    public string ComentarioAdministrador { get; set; }

    [Display(Name = "Estado")]
    [Required(ErrorMessage = "Debe de seleccionar una opción.")]
    public string Estado { get; set; }

    [Display(Name = "Monto")]
    [RequiredIf("Estado", "Aprobada", ErrorMessage = "El monto es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto asignado debe ser mayor a 0.")]
    public decimal? Amount { get; set; }

    [Display(Name = "Moneda")]
    [RequiredIf("Estado", "Aprobada", ErrorMessage = "La moneda es obligatoria.")]
    public Currency? Currency { get; set; }

    [Display(Name = "Frecuencia")]
    [RequiredIf("Estado", "Aprobada", ErrorMessage = "La frecuencia es obligatoria.")]
    public ScholarshipFrequency? Frequency { get; set; }

    [RequiredIf("Estado", "Aprobada", ErrorMessage = "La fecha de inicio es obligatoria.")]
    [MinYearsAgo(10, "La fecha de inicio no puede ser anterior a 10 años atrás.")]
    [Display(Name = "Fecha de inicio")]
    public DateTime? StartDate { get; set; }

    [RequiredIf("Estado", "Aprobada", ErrorMessage = "La fecha de finalización es obligatoria.")]
    [DateGreaterThan(nameof(StartDate), "La fecha de fin debe ser mayor que la fecha de inicio.")]
    [Display(Name = "Fecha de fin")]
    public DateTime? EndDate { get; set; }

    public IEnumerable<SelectListItem> CurrencyList { get; set; } = EnumHelper.ToSelectListItems<Currency>();
    public IEnumerable<SelectListItem> FrequencyList { get; set; } = EnumHelper.ToSelectListItems<ScholarshipFrequency>();
}

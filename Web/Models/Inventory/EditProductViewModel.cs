using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Web.Models.Inventory
{
    public class EditProductViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        [Display(Name = "Nombre del producto", Description = "Nombre del producto.")]
        public string Name { get; set; }

        [Display(Name = "Unidades de medida", Description = "Lista de unidades de medida disponibles.")]
        public List<SelectListItem> UnitsOfMeasure { get; set; } = [];

        [Required(ErrorMessage = "La unidad de medida es obligatoria.")]
        [Display(Name = "Unidad de medida", Description = "Unidad de medida seleccionada.")]
        public UnitOfMeasure? SelectedUnitOfMeasure { get; set; }

        [Required(ErrorMessage = "El stock mínimo es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "El stock mínimo debe ser un valor positivo.")]
        [Display(Name = "Stock mínimo", Description = "Cantidad mínima de stock permitida.")]
        public decimal MinimumStock { get; set; }
    }
}

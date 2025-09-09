using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Web.Models.Inventory
{
    public class RegisterProductViewModel
    {
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

        [Required(ErrorMessage = "La cantidad inicial es obligatoria.")]
        [Range(0, double.MaxValue, ErrorMessage = "La cantidad inicial debe ser un valor positivo.")]
        [Display(Name = "Cantidad inicial", Description = "Cantidad inicial del producto.")]
        public decimal InitialQuantity { get; set; }

        [StringLength(500, ErrorMessage = "El comentario no puede exceder los 500 caracteres.")]
        [Display(Name = "Comentario", Description = "Comentario adicional sobre el producto.")]
        public string Comment { get; set; }
    }
}

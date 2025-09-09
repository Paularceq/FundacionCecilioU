using Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Web.Models.Inventory
{
    public class InventoryMovementViewModel
    {
        // Información del producto
        [Required]
        [Display(Name = "ID de Producto")]
        public int ProductId { get; set; }

        [Required]
        [Display(Name = "Nombre del Producto")]
        public string ProductName { get; set; }

        [Required]
        [Display(Name = "Unidad de Medida")]
        public UnitOfMeasure UnitOfMeasure { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "El stock mínimo debe ser no negativo.")]
        [Display(Name = "Stock Mínimo")]
        public decimal MinimumStock { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "El stock actual debe ser no negativo.")]
        [Display(Name = "Stock Actual")]
        public decimal CurrentStock { get; set; }

        // Movimiento de inventario
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor que cero.")]
        [Display(Name = "Cantidad")]
        public decimal Quantity { get; set; }

        [StringLength(250, ErrorMessage = "El comentario no puede exceder los 250 caracteres.")]
        [Display(Name = "Comentario")]
        public string Comment { get; set; }
    }
}

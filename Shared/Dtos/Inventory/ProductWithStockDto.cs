using Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.Inventory
{
    public class ProductWithStockDto
    {
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Unidad de medida")]
        public UnitOfMeasure UnitOfMeasure { get; set; }

        [Display(Name = "Stock mínimo")]
        public decimal MinimumStock { get; set; }

        [Display(Name = "Stock actual")]
        public decimal CurrentStock { get; set; }

        [Display(Name = "¿Stock bajo?")]
        public bool IsLowStock { get; set; }
    }
}

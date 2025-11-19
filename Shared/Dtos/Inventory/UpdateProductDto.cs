using Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.Inventory
{
    public class UpdateProductDto
    {
        [Required(ErrorMessage = "El campo Id es obligatorio.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "La unidad de medida es obligatoria.")]
        public UnitOfMeasure? UnitOfMeasure { get; set; }

        [Required(ErrorMessage = "El stock mínimo es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo.")]
        public decimal MinimumStock { get; set; }
    }
}

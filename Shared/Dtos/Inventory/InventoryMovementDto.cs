using Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.Inventory
{
    public class InventoryMovementDto
    {
        public int Id { get; set; }

        [Display(Name = "Tipo de movimiento")]
        public MovementType Type { get; set; }

        [Display(Name = "Cantidad")]
        public decimal Quantity { get; set; }

        [Display(Name = "Comentario")]
        public string Comment { get; set; }

        [Display(Name = "Fecha")]
        public DateTime Date { get; set; }
    }
}

using Shared.Enums;

namespace Web.Models.Inventory
{
    public class InventoryMovementViewModel
    {
        // Información del producto
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public UnitOfMeasure UnitOfMeasure { get; set; }
        public decimal MinimumStock { get; set; }
        public decimal CurrentStock { get; set; }

        // Movimiento de inventario
        public decimal Quantity { get; set; }
        public string Comment { get; set; }
    }
}

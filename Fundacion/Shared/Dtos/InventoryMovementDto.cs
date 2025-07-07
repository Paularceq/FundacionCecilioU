namespace Shared.Dtos
{
    public class InventoryMovementDto
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public string Comment { get; set; }
    }
}

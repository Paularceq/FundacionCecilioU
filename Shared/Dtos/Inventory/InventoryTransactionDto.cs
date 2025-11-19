namespace Shared.Dtos.Inventory
{
    public class InventoryTransactionDto
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public string Comment { get; set; }
    }
}

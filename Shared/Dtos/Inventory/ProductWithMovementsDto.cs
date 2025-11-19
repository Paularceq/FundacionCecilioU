namespace Shared.Dtos.Inventory
{
    public class ProductWithMovementsDto : ProductWithStockDto
    {
        public IEnumerable<InventoryMovementDto> Movements { get; set; } = [];
    }
}

namespace Api.Database.Entities
{
    public class ProductsDonation
    {
        public int Id { get; set; }
        public ICollection<InventoryMovement> InventoryMovements { get; set; }

        

    }
}

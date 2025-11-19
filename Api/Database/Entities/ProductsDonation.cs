namespace Api.Database.Entities
{
    public class ProductsDonation
    {
        public int Id { get; set; }

        public int DonationId { get; set; } // FK explícita
        public Donation Donation { get; set; } // Navegación

        public ICollection<InventoryMovement> InventoryMovements { get; set; }
    }
}

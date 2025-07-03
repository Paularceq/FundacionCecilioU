namespace Api.Database.Entities
{
    public class ProductsDonation
    {
        public int Id { get; set; }
        public ICollection<DonationProduct> Products { get; set; }

    }
}

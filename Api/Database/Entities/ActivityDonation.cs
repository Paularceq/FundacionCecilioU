namespace Api.Database.Entities
{
    public class ActivityDonation
    {
        public int Id { get; set; }

        public string ActivityType { get; set; }
        public double Hours { get; set; }

        // Clave foránea explícita
        public int DonationId { get; set; }

        // Navegación
        public Donation Donation { get; set; }
    }
}

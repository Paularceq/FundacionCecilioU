using Shared.Enums;

namespace Api.Database.Entities
{
    public class MonetaryDonation
    {
        public int Id { get; set; }

        public int DonationId { get; set; }
        public Donation Donation { get; set; } // Navegación

        public double Ammount { get; set; }
        public Currency Currency { get; set; }
    }
}

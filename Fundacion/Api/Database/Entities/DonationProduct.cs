using Shared.Enums;

namespace Api.Database.Entities
{
    public class DonationProduct
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public UnitofMeasurement Unit { get; set; }
    }
}

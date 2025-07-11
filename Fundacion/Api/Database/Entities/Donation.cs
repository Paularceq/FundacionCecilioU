using Shared.Enums;

namespace Api.Database.Entities
{
    public class Donation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IdentificacionNumber { get; set; }
        public DonationType Type { get; set; }

    }
}


using Shared.Enums;

namespace Shared.Dtos.Donations
{
    public class DonationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IdentificationNumber { get; set; }
        public DonationType Type { get; set; }

        public double? Amount { get; set; }
        public Currency? Currency { get; set; }


        public String TypeDescription
        {
            get
            {
                return Type switch
                {
                    DonationType.Monetary => "Monetaria",
                    DonationType.InKind => "En Especie",
                    _ => "Desconocido"
                };
            }
        }
    }
}

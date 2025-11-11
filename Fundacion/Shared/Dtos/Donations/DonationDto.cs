using System.ComponentModel.DataAnnotations;
using Shared.Enums;

namespace Shared.Dtos.Donations
{
    public class DonationDto
    {
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        public string Name { get; set; }

        [Display(Name = "Número de Identificación")]
        public string IdentificationNumber { get; set; }

        [Display(Name = "Tipo de Donación")]
        public DonationType Type { get; set; }

        [Display(Name = "Monto")]
        public double? Amount { get; set; }

        [Display(Name = "Moneda")]
        public Currency? Currency { get; set; }

        [Display(Name = "Descripción del Tipo")]
        public string TypeDescription
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

using Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.OutgoingDonations
{
    public class InKindProductDto
    {
        [Display(Name = "ID de producto")]
        public int ProductId { get; set; }

        [Display(Name = "Nombre")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Unidad de medida")]
        public UnitOfMeasure UnitOfMeasure { get; set; }

        [Display(Name = "Cantidad solicitada")]
        public decimal RequestedQuantity { get; set; }

        [Display(Name = "Stock actual")]
        public decimal CurrentStock { get; set; }
    }
}

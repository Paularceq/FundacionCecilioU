using Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Database.Entities
{
    public class InventoryMovement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;

        [Required]
        public MovementType Type { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        [MaxLength(500)]
        public string Comment { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.Now;

        [Required]
        public RequestStatus Status { get; set; }

        public int? ProductsDonationId { get; set; } = null;
    }

}

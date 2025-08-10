using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Database.Entities
{
    public class Lease
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("FinancialMovement")]
        public int FinancialMovementId { get; set; }

        [Required]
        public FinancialMovement FinancialMovement { get; set; }

        [Required]
        [StringLength(100)]
        public string TenantName { get; set; }

        [Required]
        [StringLength(50)]
        public string TenantIdentification { get; set; }

        [Required]
        [StringLength(50)]
        public string Usage { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string ReceiptBytes { get; set; }

        [StringLength(100)]
        public string ReceiptContentType { get; set; }

        [StringLength(255)]
        public string ReceiptFileName { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Database.Entities
{
    public class Expense
    {
        public int Id { get; set; }
        public int FinancialMovementId { get; set; }
        public FinancialMovement FinancialMovement { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "nvarchar(max)")]
        public string ReceiptBytes { get; set; }

        [StringLength(100)]
        public string ReceiptContentType { get; set; }

        [StringLength(255)]
        public string ReceiptFileName { get; set; }
    }
}

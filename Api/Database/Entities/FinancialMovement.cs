using Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Database.Entities
{
    public class FinancialMovement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public Currency Currency { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public MovementType Type { get; set; }

        [ForeignKey("CreatedBy")]
        public int CreatedById { get; set; }

        [Required]
        public User CreatedBy { get; set; }
    }
}

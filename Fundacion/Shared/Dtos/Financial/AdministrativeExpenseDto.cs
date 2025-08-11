using Shared.Enums;

namespace Shared.Dtos.Financial
{
    public class AdministrativeExpenseDto
    {
        public int CreatedById { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string ReceiptBytes { get; set; }
        public string ReceiptContentType { get; set; }
        public string ReceiptFileName { get; set; }
    }
}

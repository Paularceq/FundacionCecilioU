using Shared.Enums;

namespace Shared.Dtos.Financial
{
    public class LeaseIncomeDto
    {
        public int CreatedById { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public string TenantName { get; set; } 
        public string TenantIdentification { get; set; }
        public string Usage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ReceiptBytes { get; set; }
        public string ReceiptContentType { get; set; }
        public string ReceiptFileName { get; set; }
    }
}

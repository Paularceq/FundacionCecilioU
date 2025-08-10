using Shared.Enums;

namespace Web.Models.Financial
{
    public class LeaseIncomeViewModel
    {
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public string TenantName { get; set; }
        public string TenantIdentification { get; set; }
        public string Usage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IFormFile ReceiptFile { get; set; }
    }
}

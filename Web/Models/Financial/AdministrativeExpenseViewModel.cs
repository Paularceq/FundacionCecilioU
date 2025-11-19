using Shared.Enums;

namespace Web.Models.Financial
{
    public class AdministrativeExpenseViewModel
    {
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public IFormFile ReceiptFile { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.Dtos;
using Shared.Dtos.Inventory;
using Shared.Dtos.OutgoingDonations;

namespace Web.Models.OutgoingDonation
{
    public class CreateOutgoingDonationViewModel
    {
        public IEnumerable<SelectListItem> Products { get; set; } = [];
        public InKindItemDto[] SelectedProducts { get; set; } = [];
        public IEnumerable<SelectListItem> Students { get; set; } = [];
        public int SelectedStudentId { get; set; }
    }
}

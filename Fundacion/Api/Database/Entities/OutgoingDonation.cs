using Shared.Enums;

namespace Api.Database.Entities
{
    public class OutgoingDonation
    {
        public int Id { get; set; }
        public int RequesterId { get; set; }
        public User Requester { get; set; }
        public int RecipientId { get; set; }
        public User Recipient { get; set; }
        public int? ApproverId { get; set; }
        public User Approver { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DonationType Type { get; set; }

        public double Ammount { get; set; }
        public Currency Currency { get; set; }

        public ICollection<InventoryMovement> InventoryMovements { get; set; }
    }
}

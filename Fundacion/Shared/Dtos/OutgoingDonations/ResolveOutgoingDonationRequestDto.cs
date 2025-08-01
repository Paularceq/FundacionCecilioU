namespace Shared.Dtos.OutgoingDonations
{
    public class ResolveOutgoingDonationRequestDto
    {
        public int DonationId { get; set; }
        public int ApproverId { get; set; }
        public bool IsApproved { get; set; }
    }
}

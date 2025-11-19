namespace Shared.Dtos.OutgoingDonations
{
    public class CreateInKindDonationDto
    {
        public int RequesterId { get; set; }
        public int RecipientId { get; set; }
        public List<InKindItemDto> Products { get; set; }
    }
}

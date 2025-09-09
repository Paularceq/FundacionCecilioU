namespace Shared.Dtos.OutgoingDonations
{
    public class OutgoingDonationDetailsDto : OutgoingDonationToListDto
    {
        public List<InKindProductDto> Products { get; set; } = [];
    }
}

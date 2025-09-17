using Shared.Enums;

namespace Shared.Dtos.PublicContent
{
    public class SubscriptionDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime SubscriptionDate { get; set; }
        public SubscriptionFrequency Frequency { get; set; }
    }
}
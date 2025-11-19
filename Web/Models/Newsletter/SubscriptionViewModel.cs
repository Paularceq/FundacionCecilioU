namespace Web.Models.Newsletter
{
    public class SubscriptionViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime SubscriptionDate { get; set; }
        public string Frequency { get; set; } = string.Empty;

        // Para mostrar en la vista
        public string StatusBadge => IsActive ? "Activo" : "Inactivo";
        public string StatusClass => IsActive ? "badge-success" : "badge-secondary";
        public string FrequencyText => Frequency switch
        {
            "Daily" => "Diario",
            "Weekly" => "Semanal",
            "Monthly" => "Mensual",
            _ => Frequency
        };
    }
}

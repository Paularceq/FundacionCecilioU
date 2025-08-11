namespace Web.Models.Newsletter
{
    public class HomeContentViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }

        // Para mostrar en la vista
        public string StatusBadge => IsActive ? "Activo" : "Inactivo";
        public string StatusClass => IsActive ? "badge-success" : "badge-secondary";
        public bool IsCurrentlyActive => IsActive &&
            (StartDate == null || StartDate <= DateTime.Now) &&
            (EndDate == null || EndDate >= DateTime.Now);
    }
}
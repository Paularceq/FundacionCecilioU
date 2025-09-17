using Shared.Enums;

namespace Web.Models.Newsletter
{
    public class NewsletterViewModel
    {
        public int Id { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string CustomContent { get; set; } = string.Empty;
        public DateTime? SendDate { get; set; }
        public NewsletterStatus Status { get; set; }
        public int RecipientCount { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }

        // Para mostrar en la vista
        public string StatusText => Status switch
        {
            NewsletterStatus.Draft => "Borrador",
            NewsletterStatus.Scheduled => "Programado",
            NewsletterStatus.Sending => "Enviando",
            NewsletterStatus.Sent => "Enviado",
            NewsletterStatus.Failed => "Falló",
            _ => "Desconocido"
        };

        public string StatusClass => Status switch
        {
            NewsletterStatus.Draft => "badge-secondary",
            NewsletterStatus.Scheduled => "badge-warning",
            NewsletterStatus.Sending => "badge-info",
            NewsletterStatus.Sent => "badge-success",
            NewsletterStatus.Failed => "badge-danger",
            _ => "badge-secondary"
        };

        public bool CanBeSent => Status == NewsletterStatus.Draft;
        public bool CanBeDeleted => Status != NewsletterStatus.Sent;
    }
}

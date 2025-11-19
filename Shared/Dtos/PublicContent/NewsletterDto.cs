using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.PublicContent
{
    public class NewsletterDto
    {
        public int Id { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string CustomContent { get; set; } = string.Empty;
        public DateTime? SendDate { get; set; }
        public NewsletterStatus Status { get; set; }
        public int RecipientCount { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}

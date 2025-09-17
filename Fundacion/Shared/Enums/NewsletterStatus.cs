using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Enums
{
    public enum NewsletterStatus
    {
        Draft = 1,
        Scheduled = 2,
        Sending = 3,
        Sent = 4,
        Failed = 5
    }
}

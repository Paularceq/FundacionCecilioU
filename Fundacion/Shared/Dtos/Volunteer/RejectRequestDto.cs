using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.Volunteer
{
    public class RejectRequestDto
    {
        public int ApproverId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

}

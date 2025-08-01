using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.Volunteer
{
    public class ApproveRejectHoursDto
    {
        [Required]
        public int HoursId { get; set; }

        [Required]
        public bool IsApproved { get; set; }

        public string? RejectionReason { get; set; }
        public int ApproverId { get; set; }
        public string ApproverName { get; set; } = string.Empty;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.Donations
{
    public class AddActivityDonationDto
    {
        public string Name { get; set; }
        public string Identification { get; set; }

        public string ActivityType { get; set; }
        public double Hours { get; set; }
    }
}

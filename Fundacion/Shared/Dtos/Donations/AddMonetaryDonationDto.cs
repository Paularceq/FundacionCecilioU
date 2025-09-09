using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.Donations
{
    public class AddMonetaryDonationDto
    {
        public string Name { get; set; }
        public string Identification { get; set; }
        public double Amount { get; set; }
        public Currency Currency { get; set; }
        public int CreatedById { get; set; }

    }
}

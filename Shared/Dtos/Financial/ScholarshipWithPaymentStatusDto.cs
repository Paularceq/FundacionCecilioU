using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.Financial
{
    public class ScholarshipWithPaymentStatusDto
    {
        public int Id { get; set; }
        public string NombreEstudiante { get; set; }
        public string CedulaEstudiante { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public ScholarshipFrequency Frequency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? LastPayment { get; set; }
        public bool IsActive { get; set; }
        public bool IsPendingPayment { get; set; }
    }
}

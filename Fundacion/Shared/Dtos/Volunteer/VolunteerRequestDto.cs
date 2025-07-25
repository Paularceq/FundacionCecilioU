using Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.Volunteer
{
    public class VolunteerRequestDto
    {
        [Display(Name = "Id")]
        public int Id { get; set; }
        [Display(Name = "Solicitante Id")]
        public int VolunteerId { get; set; }
        [Display(Name = "Solicitante")]
        public string VolunteerName { get; set; }
        [Display(Name = "Aprobador Id")]
        public int? ApproverId { get; set; }
        [Display(Name = "Aprobador")]
        public string ApproverName { get; set; }
        [Display(Name = "Fecha de Creación")]
        public DateTime CreatedAt { get; set; }
        [Display(Name = "Estado")]
        public VolunteerState State { get; set; }
        [Display(Name = "Institución")]
        public string Institution { get; set; }
        [Display(Name = "Profesión")]
        public string Profession { get; set; }
        [Display(Name = "Descripción")]
        public string Description { get; set; }
        [Display(Name = "Horas")]
        public int Hours { get; set; }
    }
}

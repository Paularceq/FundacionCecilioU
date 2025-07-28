using Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Web.Models.Volunteer
{
    public class CreateRequestViewModel
    {
     
            [Required]
            [Display(Name = "Institucion")]
            public string Institution { get; set; }

            [Required]
            [Display(Name = "Profesion")]
            public string Profession { get; set; }

            [Required]
            [Display(Name = "Descripcion del trabajo")]
            public string Description { get; set; }

            [Required]            
            [Display(Name = "Horas")]
            public int Hours { get; set; }
        
    }
}

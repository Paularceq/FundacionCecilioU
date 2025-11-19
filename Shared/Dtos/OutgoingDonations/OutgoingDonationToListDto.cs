using Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.OutgoingDonations
{
    public class OutgoingDonationToListDto
    {
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Display(Name = "Solicitante")]
        public string RequesterName { get; set; }

        [Display(Name = "Destinatario")]
        public string RecipientName { get; set; }

        [Display(Name = "Aprobador")]
        public string ApproverName { get; set; }

        [Display(Name = "Estado")]
        public RequestStatus Status { get; set; }

        [Display(Name = "Fecha de Solicitud")]
        public DateTime RequestDate { get; set; }

        [Display(Name = "Fecha de Aprobación")]
        public DateTime? ApprovalDate { get; set; }

        [Display(Name = "Tipo de Donación")]
        public DonationType Type { get; set; }

        public string StatusDescription
        {
            get
            {
                return Status switch
                {
                    RequestStatus.Pending => "Pendiente",
                    RequestStatus.Approved => "Aprobada",
                    RequestStatus.Rejected => "Rechazada",
                    _ => "Desconocido"
                };
            }
        }

        public string TypeDescription
        {
            get
            {
                return Type switch
                {
                    DonationType.Monetary => "Monetaria",
                    DonationType.InKind => "En Especie",
                    _ => "Desconocido"
                };
            }
        }
    }
}

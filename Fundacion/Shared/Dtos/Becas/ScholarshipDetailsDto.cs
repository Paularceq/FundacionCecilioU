using Shared.Enums;

namespace Shared.Dtos.Becas
{
    public class ScholarshipDetailsDto
    {
        // Datos de la beca
        public int ScholarshipId { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public ScholarshipFrequency Frequency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? LastPayment { get; set; }
        public bool IsActive { get; set; }

        // Datos de la solicitud
        public int SolicitudId { get; set; }
        public string CedulaEstudiante { get; set; }
        public string NombreEstudiante { get; set; }
        public string CorreoContacto { get; set; }
        public string TelefonoContacto { get; set; }
        public string Direccion { get; set; }
        public string Colegio { get; set; }
        public string NivelEducativo { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public string Estado { get; set; }
        public string ComentarioAdministrador { get; set; }
    }
}
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.ComponentModel.DataAnnotations;


namespace Api.Database.Entities
{
    public class SolicitudBeca
    {
        [Key]
        public int Id { get; set; }

        //Informacion del estudiante 

        [Required]
        [StringLength(20)]
        public string CedulaEstudiante { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreEstudiante { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string CorreoContacto { get; set; }

        [StringLength(20)]
        public string TelefonoContacto { get; set; }

        [StringLength(250)]
        public string Direccion { get; set; }

        [Required]
        [StringLength(100)]
        public string Colegio { get; set; }

        [Required]
        [StringLength(50)]
        public string NivelEducativo { get; set; }

        // Archivos adjuntos 

        public byte[]? CartaConsentimiento { get; set; }
        public byte[]? CartaNotas { get; set; }

        public string? CartaConsentimientoContentType { get; set; }
        public string? CartaNotasContentType { get; set; }

        //Otros campos
        public DateTime FechaSolicitud { get; set; } = DateTime.UtcNow;

        public EstadoSolicitud Estado { get; set; } = EstadoSolicitud.Pendiente;

        [Range(0,double.MaxValue)]
        public decimal? MontoAsignado { get; set; }

        [StringLength(500)]
        public string? ComentarioAdministrador { get; set; }
        public bool EsFormularioManual { get; set; }
    }

    public enum EstadoSolicitud
    {
        Pendiente =0,
        Aprobada = 1,
        Rechazada = 2
    }
   }

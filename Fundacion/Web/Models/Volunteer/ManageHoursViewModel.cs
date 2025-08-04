using Shared.Dtos.Volunteer;

namespace Web.Models.Volunteer
{
    public class ManageHoursViewModel
    {
        public int RequestId { get; set; }
        public string VolunteerName { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
        public List<VolunteerHoursDto> HoursList { get; set; } = new();
        public bool CanAddMore { get; set; } = true;

        // Horas solicitadas desde la solicitud original
        public decimal TotalHoursRequested { get; set; }

        // Estadísticas básicas según requerimientos
        public decimal TotalHoursWorked => HoursList.Sum(h => h.TotalHours);

        public decimal TotalHoursApproved => HoursList
            .Where(h => h.State == Shared.Enums.VolunteerState.Approved)
            .Sum(h => h.TotalHours);

        public decimal TotalHoursPending => HoursList
            .Where(h => h.State == Shared.Enums.VolunteerState.Pending)
            .Sum(h => h.TotalHours);

        public int TotalDaysWorked => HoursList
            .Where(h => h.State == Shared.Enums.VolunteerState.Approved)
            .Select(h => h.Date.Date)
            .Distinct()
            .Count();

        // Horas restantes (calculadas) - REQUERIMIENTO CLAVE
        public decimal RemainingHours => TotalHoursRequested - TotalHoursApproved;

        // Porcentaje de progreso
        public decimal ProgressPercentage => TotalHoursRequested > 0
            ? Math.Round((TotalHoursApproved / TotalHoursRequested) * 100, 1)
            : 0;

        // Filtros básicos
        public DateTime? FilterStartDate { get; set; }
        public DateTime? FilterEndDate { get; set; }
        public string? FilterState { get; set; }

        // Propiedades de utilidad para las vistas
        public bool HasHours => HoursList.Any();
        public bool HasApprovedHours => TotalHoursApproved > 0;
        public bool HasPendingHours => TotalHoursPending > 0;
        public bool IsCompleted => RemainingHours <= 0;
        public string StatusText => IsCompleted ? "Completado" : $"{RemainingHours:F1}h restantes";
        public string StatusCssClass => IsCompleted ? "text-success" : "text-warning";
    }
}
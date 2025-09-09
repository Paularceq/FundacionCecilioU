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

        // ✅ REQUERIMIENTO 1: VALIDACIÓN DE HORAS RESTANTES
        public decimal RemainingHours => TotalHoursRequested - TotalHoursApproved;

        // ✅ AGREGADO: Validación para registro de nuevas horas
        public bool CanRegisterHours(decimal hoursToRegister)
        {
            return hoursToRegister <= RemainingHours && RemainingHours > 0;
        }

        // ✅ AGREGADO: Mensaje de validación para la vista
        public string GetHoursValidationMessage(decimal hoursToRegister)
        {
            if (RemainingHours <= 0)
                return "Ya has completado todas las horas comprometidas para esta solicitud.";

            if (hoursToRegister > RemainingHours)
                return $"No puedes registrar {hoursToRegister} horas. Solo quedan {RemainingHours:F1} horas disponibles.";

            return string.Empty;
        }

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

        // ✅ REQUERIMIENTO 3: Helper para manejar estados de re-registro
        public bool CanEditOrDeleteHours(VolunteerHoursDto hours)
        {
            return hours.State == Shared.Enums.VolunteerState.Pending ||
                   hours.State == Shared.Enums.VolunteerState.Rejected;
        }

        // ✅ AGREGADO: Información sobre registros rechazados para la vista
        public List<VolunteerHoursDto> RejectedHours => HoursList
            .Where(h => h.State == Shared.Enums.VolunteerState.Rejected)
            .ToList();

        public bool HasRejectedHours => RejectedHours.Any();
    }
}
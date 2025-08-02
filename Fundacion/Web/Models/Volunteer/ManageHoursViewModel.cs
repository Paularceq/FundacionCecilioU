using Shared.Dtos.Volunteer;

namespace Web.Models.Volunteer
{
    public class ManageHoursViewModel
    {
        public int RequestId { get; set; }
        public string VolunteerName { get; set; } = string.Empty;
        public List<VolunteerHoursDto> HoursList { get; set; } = new();
        public bool CanAddMore { get; set; } = true;

        // Horas solicitadas desde la solicitud original
        public decimal TotalHoursRequested { get; set; }

        // Estadísticas rápidas
        public decimal TotalHoursWorked => HoursList.Sum(h => h.TotalHours);
        public decimal TotalHoursApproved => HoursList
            .Where(h => h.State == Shared.Enums.VolunteerState.Approved)
            .Sum(h => h.TotalHours);
        public decimal TotalHoursPending => HoursList
            .Where(h => h.State == Shared.Enums.VolunteerState.Pending)
            .Sum(h => h.TotalHours);
        public int TotalDaysWorked => HoursList
            .Select(h => h.Date.Date)
            .Distinct()
            .Count();

        // Horas restantes (calculadas)
        public decimal RemainingHours => TotalHoursRequested - TotalHoursApproved;

        // Filtros
        public DateTime? FilterStartDate { get; set; }
        public DateTime? FilterEndDate { get; set; }
        public string? FilterState { get; set; }

        // Paginación
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalRecords => HoursList.Count;
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);

        // Datos para gráficos
        public string WeeklyHoursData
        {
            get
            {
                var weeklyData = HoursList
                    .Where(h => h.State == Shared.Enums.VolunteerState.Approved)
                    .GroupBy(h => GetWeekOfYear(h.Date))
                    .OrderBy(g => g.Key)
                    .Take(8) // Últimas 8 semanas
                    .Select(g => g.Sum(h => h.TotalHours))
                    .ToArray();

                return "[" + string.Join(",", weeklyData.Select(d => d.ToString("F1"))) + "]";
            }
        }

        private static int GetWeekOfYear(DateTime date)
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            return culture.Calendar.GetWeekOfYear(date,
                culture.DateTimeFormat.CalendarWeekRule,
                culture.DateTimeFormat.FirstDayOfWeek);
        }
    }
}

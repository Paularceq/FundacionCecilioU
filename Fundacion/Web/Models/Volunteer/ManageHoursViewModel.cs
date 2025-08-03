using Shared.Dtos.Volunteer;

namespace Web.Models.Volunteer
{
    public class ManageHoursViewModel
    {
        public int RequestId { get; set; }
        public string VolunteerName { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty; // ✅ AGREGADO: Faltaba esta propiedad
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

        public decimal TotalHoursRejected => HoursList
            .Where(h => h.State == Shared.Enums.VolunteerState.Rejected)
            .Sum(h => h.TotalHours);

        public int TotalDaysWorked => HoursList
            .Select(h => h.Date.Date)
            .Distinct()
            .Count();

        // Horas restantes (calculadas)
        public decimal RemainingHours => TotalHoursRequested - TotalHoursApproved;

        // Porcentajes para gráficos
        public decimal ProgressPercentage => TotalHoursRequested > 0
            ? Math.Round((TotalHoursApproved / TotalHoursRequested) * 100, 1)
            : 0;

        public decimal PendingPercentage => TotalHoursRequested > 0
            ? Math.Round((TotalHoursPending / TotalHoursRequested) * 100, 1)
            : 0;

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

        public string MonthlyHoursData
        {
            get
            {
                var monthlyData = HoursList
                    .Where(h => h.State == Shared.Enums.VolunteerState.Approved)
                    .GroupBy(h => new { h.Date.Year, h.Date.Month })
                    .OrderBy(g => new DateTime(g.Key.Year, g.Key.Month, 1))
                    .Take(6) // Últimos 6 meses
                    .Select(g => g.Sum(h => h.TotalHours))
                    .ToArray();

                return "[" + string.Join(",", monthlyData.Select(d => d.ToString("F1"))) + "]";
            }
        }

        public string StateDistributionData
        {
            get
            {
                var approved = TotalHoursApproved;
                var pending = TotalHoursPending;
                var rejected = TotalHoursRejected;

                return $"[{approved:F1},{pending:F1},{rejected:F1}]";
            }
        }

        // Datos para las etiquetas de los gráficos
        public string WeekLabels
        {
            get
            {
                var labels = HoursList
                    .Where(h => h.State == Shared.Enums.VolunteerState.Approved)
                    .GroupBy(h => GetWeekOfYear(h.Date))
                    .OrderBy(g => g.Key)
                    .Take(8)
                    .Select(g => $"\"Semana {g.Key}\"")
                    .ToArray();

                return "[" + string.Join(",", labels) + "]";
            }
        }

        public string MonthLabels
        {
            get
            {
                var labels = HoursList
                    .Where(h => h.State == Shared.Enums.VolunteerState.Approved)
                    .GroupBy(h => new { h.Date.Year, h.Date.Month })
                    .OrderBy(g => new DateTime(g.Key.Year, g.Key.Month, 1))
                    .Take(6)
                    .Select(g => $"\"{GetMonthName(g.Key.Month)} {g.Key.Year}\"")
                    .ToArray();

                return "[" + string.Join(",", labels) + "]";
            }
        }

        // Métodos auxiliares
        private static int GetWeekOfYear(DateTime date)
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            return culture.Calendar.GetWeekOfYear(date,
                culture.DateTimeFormat.CalendarWeekRule,
                culture.DateTimeFormat.FirstDayOfWeek);
        }

        private static string GetMonthName(int month)
        {
            return month switch
            {
                1 => "Ene",
                2 => "Feb",
                3 => "Mar",
                4 => "Abr",
                5 => "May",
                6 => "Jun",
                7 => "Jul",
                8 => "Ago",
                9 => "Sep",
                10 => "Oct",
                11 => "Nov",
                12 => "Dic",
                _ => "N/A"
            };
        }

        // Propiedades de utilidad para las vistas
        public bool HasHours => HoursList.Any();
        public bool HasApprovedHours => TotalHoursApproved > 0;
        public bool HasPendingHours => TotalHoursPending > 0;
        public bool IsCompleted => RemainingHours <= 0;
        public string StatusText => IsCompleted ? "Completado" : $"{RemainingHours:F1}h restantes";
        public string StatusCssClass => IsCompleted ? "text-success" : "text-warning";
    }
}
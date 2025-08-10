using Api.Abstractions.Application;
using Api.Database;
using Api.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;
using Shared.Models;

namespace Api.Services.Application
{
    public class ScholarshipPaymentService : IScholarshipPaymentService
    {
        private readonly DatabaseContext _db;

        public ScholarshipPaymentService(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<Result<string>> ProcessPendingScholarshipsAsync(int userId)
        {
            var now = DateTime.UtcNow;
            var firstDay = new DateTime(now.Year, now.Month, 1);
            var lastDay = firstDay.AddMonths(1).AddDays(-1);

            var scholarships = await _db.Scholarships
                .Include(s => s.Request)
                .Where(s => s.IsActive
                    && s.StartDate <= lastDay
                    && (s.EndDate == null || s.EndDate >= firstDay)
                    && (
                        s.Frequency == ScholarshipFrequency.OneTime && s.LastPayment == null
                        || s.Frequency == ScholarshipFrequency.Monthly && (s.LastPayment == null || s.LastPayment.Value.Month != now.Month || s.LastPayment.Value.Year != now.Year)
                        || s.Frequency == ScholarshipFrequency.Quarterly && (s.LastPayment == null || s.LastPayment.Value < firstDay.AddMonths(-3))
                        || s.Frequency == ScholarshipFrequency.Semiannual && (s.LastPayment == null || s.LastPayment.Value < firstDay.AddMonths(-6))
                        || s.Frequency == ScholarshipFrequency.Annual && (s.LastPayment == null || s.LastPayment.Value.Year != now.Year)
                    )
                )
                .ToListAsync();

            var pagadas = new List<Scholarship>();

            foreach (var scholarship in scholarships)
            {
                // Registrar movimiento financiero
                var movement = new FinancialMovement
                {
                    Description = $"Pago de beca a {scholarship.Request.NombreEstudiante} ({scholarship.Request.CedulaEstudiante})",
                    Amount = scholarship.Amount,
                    Currency = scholarship.Currency,
                    Date = now,
                    Type = MovementType.Outbound,
                    CreatedById = userId
                };

                _db.FinancialMovements.Add(movement);

                // Actualizar último pago
                scholarship.LastPayment = now;

                // Marcar como inactiva si el periodo está vencido o es de una sola vez
                if ((scholarship.EndDate != null && scholarship.EndDate < now) ||
                    scholarship.Frequency == ScholarshipFrequency.OneTime)
                {
                    scholarship.IsActive = false;
                }

                pagadas.Add(scholarship);
            }

            await _db.SaveChangesAsync();

            if (pagadas.Count <= 0)
            {
                return Result<string>.Failure("No había becas pendientes de pago para este mes.");
            }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Becas pagadas este mes:");
            foreach (var s in pagadas)
            {
                sb.AppendLine($"- {s.Request.NombreEstudiante} ({s.Request.CedulaEstudiante}), Monto: {s.Amount} {s.Currency}, Frecuencia: {s.Frequency}");
            }

            return Result<string>.Success(sb.ToString());
        }
    }
}

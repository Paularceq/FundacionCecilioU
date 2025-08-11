using Api.Abstractions.Application;
using Api.Database;
using Api.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Dtos.Financial;
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

        public async Task<List<ScholarshipWithPaymentStatusDto>> GetScholarshipsWithPaymentStatusAsync()
        {
            var now = DateTime.Now;

            var scholarships = await _db.Scholarships
                .Include(s => s.Request)
                .ToListAsync();

            var result = scholarships.Select(s => new ScholarshipWithPaymentStatusDto
            {
                Id = s.Id,
                NombreEstudiante = s.Request?.NombreEstudiante,
                CedulaEstudiante = s.Request?.CedulaEstudiante,
                Amount = s.Amount,
                Currency = s.Currency,
                Frequency = s.Frequency,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                LastPayment = s.LastPayment,
                IsActive = s.IsActive,
                IsPendingPayment =
                    s.IsActive
                    && (
                        s.Frequency == ScholarshipFrequency.OneTime && s.LastPayment == null
                        || s.Frequency == ScholarshipFrequency.Monthly && (s.LastPayment == null || s.LastPayment.Value.Month != now.Month || s.LastPayment.Value.Year != now.Year)
                        || s.Frequency == ScholarshipFrequency.Quarterly && (s.LastPayment == null || s.LastPayment.Value < now.AddMonths(-3))
                        || s.Frequency == ScholarshipFrequency.Semiannual && (s.LastPayment == null || s.LastPayment.Value < now.AddMonths(-6))
                        || s.Frequency == ScholarshipFrequency.Annual && (s.LastPayment == null || s.LastPayment.Value.Year != now.Year)
                    )
            }).ToList();

            return result;
        }

        public async Task<Result<string>> ProcessPendingScholarshipsAsync(int userId)
        {
            var now = DateTime.Now;
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
                return Result<string>.Failure("No había becas pendientes de pago.");
            }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Becas pagadas:");
            sb.AppendLine("<ul>");
            foreach (var s in pagadas)
            {
                sb.AppendLine($"<li>{s.Request.NombreEstudiante} ({s.Request.CedulaEstudiante}), Monto: {s.Amount} {s.Currency}, Frecuencia: {s.Frequency}</li>");
            }
            sb.AppendLine("</ul>");

            return Result<string>.Success(sb.ToString());
        }

        public async Task<Result> ProcessScholarshipPaymentAsync(int scholarshipId, int userId)
        {
            var now = DateTime.Now;
            var scholarship = await _db.Scholarships
                .Include(s => s.Request)
                .FirstOrDefaultAsync(s => s.Id == scholarshipId);

            if (scholarship == null)
                return Result.Failure("No se encontró la beca.");

            if (!scholarship.IsActive)
                return Result.Failure("La beca no está activa.");

            // Verificar si ya fue pagada según la frecuencia
            bool alreadyPaid =
                (scholarship.Frequency == ScholarshipFrequency.OneTime && scholarship.LastPayment != null) ||
                (scholarship.Frequency == ScholarshipFrequency.Monthly && scholarship.LastPayment != null && scholarship.LastPayment.Value.Month == now.Month && scholarship.LastPayment.Value.Year == now.Year) ||
                (scholarship.Frequency == ScholarshipFrequency.Quarterly && scholarship.LastPayment != null && scholarship.LastPayment.Value >= now.AddMonths(-3)) ||
                (scholarship.Frequency == ScholarshipFrequency.Semiannual && scholarship.LastPayment != null && scholarship.LastPayment.Value >= now.AddMonths(-6)) ||
                (scholarship.Frequency == ScholarshipFrequency.Annual && scholarship.LastPayment != null && scholarship.LastPayment.Value.Year == now.Year);

            if (alreadyPaid)
                return Result.Failure("La beca ya fue pagada en el periodo correspondiente.");

            // Registrar movimiento financiero
            var movement = new FinancialMovement
            {
                Description = $"Pago de beca a {scholarship.Request?.NombreEstudiante} ({scholarship.Request?.CedulaEstudiante})",
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

            await _db.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> SetScholarshipActiveStatusAsync(int scholarshipId, bool isActive)
        {
            var scholarship = await _db.Scholarships.FirstOrDefaultAsync(s => s.Id == scholarshipId);

            if (scholarship == null)
                return Result.Failure("No se encontró la beca.");

            if (scholarship.IsActive == isActive)
                return Result.Failure(isActive ? "La beca ya está activa." : "La beca ya está inactiva.");

            scholarship.IsActive = isActive;
            await _db.SaveChangesAsync();

            return Result.Success();
        }
    }
}

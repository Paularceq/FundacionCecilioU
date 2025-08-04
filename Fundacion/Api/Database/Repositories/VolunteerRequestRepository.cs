using Api.Abstractions.Repositories;
using Api.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;

namespace Api.Database.Repositories
{
    public class VolunteerRequestRepository : IVolunteerRequestRepository
    {
        private readonly DatabaseContext _context;

        public VolunteerRequestRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<List<VolunteerRequest>> GetRequestsByVolunteerID(int volunteerID)
        {
            return await _context.VolunteerRequests
                .Include(v=>v.Volunteer)
                .Include(v=>v.Approver)
                .Where(v => v.VolunteerId == volunteerID).ToListAsync();
        }
        public async Task CreateRequest(VolunteerRequest volunteerRequest)
        {
            _context.VolunteerRequests.Add(volunteerRequest);
            await _context.SaveChangesAsync();
        }
        public async Task <VolunteerRequest> GetActiveRequest(int VolunteerId)
        {
            return await _context.VolunteerRequests
                .Where(v => v.VolunteerId == VolunteerId && (v.State == Shared.Enums.VolunteerState.Pending  || v.State == Shared.Enums.VolunteerState.Approved))
                .FirstOrDefaultAsync();

        }
        // ===== NUEVOS MÉTODOS PARA ADMINISTRACIÓN =====
        public async Task<List<VolunteerRequest>> GetAllRequestsAsync()
        {
            return await _context.VolunteerRequests
                .Include(v => v.Volunteer)
                .Include(v => v.Approver)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<VolunteerRequest>> GetRequestsByStateAsync(VolunteerState state)
        {
            return await _context.VolunteerRequests
                .Include(v => v.Volunteer)
                .Include(v => v.Approver)
                .Where(v => v.State == state)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        public async Task<VolunteerRequest?> GetRequestByIdAsync(int requestId)
        {
            return await _context.VolunteerRequests
                .Include(v => v.Volunteer)
                .Include(v => v.Approver)
                .FirstOrDefaultAsync(v => v.Id == requestId);
        }

        public async Task UpdateRequestAsync(VolunteerRequest request)
        {
            _context.VolunteerRequests.Update(request);
            await _context.SaveChangesAsync();
        }

        public async Task ApproveRequestAsync(int requestId, int approverId)
        {
            var request = await GetRequestByIdAsync(requestId);
            if (request != null)
            {
                request.State = VolunteerState.Approved;
                request.ApproverId = approverId;
                request.ApprovedAt = DateTime.UtcNow; // ← Hora de aprobación
                await UpdateRequestAsync(request);
            }
        }

        public async Task RejectRequestAsync(int requestId, int approverId, string reason)
        {
            var request = await GetRequestByIdAsync(requestId);
            if (request != null)
            {
                request.State = VolunteerState.Rejected;
                request.ApproverId = approverId;
                request.ApprovedAt = DateTime.UtcNow; // ← Hora de rechazo
                request.RejectionReason = reason; // ← Razón del rechazo 
                await UpdateRequestAsync(request);
            }
        }

        // ===== GESTIÓN DE HORAS =====
        public async Task AddVolunteerHoursAsync(VolunteerHours hours)
        {
            _context.VolunteerHours.Add(hours);
            await _context.SaveChangesAsync();
        }

        public async Task<VolunteerHours?> GetVolunteerHoursAsync(int hoursId)
        {
            return await _context.VolunteerHours
                .Include(vh => vh.VolunteerRequest)
                    .ThenInclude(vr => vr.Volunteer)
                .Include(vh => vh.Approver)
                .FirstOrDefaultAsync(vh => vh.Id == hoursId);
        }

        public async Task<List<VolunteerHours>> GetHoursByRequestIdAsync(int requestId)
        {
            return await _context.VolunteerHours
                .Include(vh => vh.Approver)
                .Where(vh => vh.VolunteerRequestId == requestId )
                .OrderByDescending(vh => vh.Date)
                .ToListAsync();
        }

        public async Task<List<VolunteerHours>> GetHoursByDateRangeAsync(int requestId, DateTime startDate, DateTime endDate)
        {
            return await _context.VolunteerHours
                .Include(vh => vh.Approver)
                .Where(vh => vh.VolunteerRequestId == requestId &&                          
                            vh.Date >= startDate &&
                            vh.Date <= endDate)
                .OrderBy(vh => vh.Date)
                .ToListAsync();
        }

        public async Task UpdateVolunteerHoursAsync(VolunteerHours hours)
        {
            _context.VolunteerHours.Update(hours);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteVolunteerHoursAsync(int hoursId)
        {
            var hours = await GetVolunteerHoursAsync(hoursId);
            if (hours != null)
            {

                _context.VolunteerHours.Remove(hours);
                await _context.SaveChangesAsync();
            }
        }

        // ===== VALIDACIONES =====
        public async Task<VolunteerHours?> GetHoursForDateAsync(int requestId, DateTime date)
        {
            return await _context.VolunteerHours
                .FirstOrDefaultAsync(vh => vh.VolunteerRequestId == requestId &&
                                          vh.Date.Date == date.Date);
        }

        public async Task<decimal> GetTotalHoursForDateAsync(int requestId, DateTime date)
        {
            return await _context.VolunteerHours
                .Where(vh => vh.VolunteerRequestId == requestId &&
                            vh.Date.Date == date.Date)
                .SumAsync(vh => vh.TotalHours);
        }

        public async Task<bool> HasHoursForDateAsync(int requestId, DateTime date)
        {
            return await _context.VolunteerHours
                .AnyAsync(vh => vh.VolunteerRequestId == requestId &&
                               vh.Date.Date == date.Date);
        }

        // ===== APROBACIÓN DE HORAS =====
        public async Task ApproveHoursAsync(int hoursId, int approverId)
        {
            var hours = await GetVolunteerHoursAsync(hoursId);
            if (hours != null)
            {
                hours.State = VolunteerState.Approved;
                hours.ApproverId = approverId;
                hours.ApprovedAt = DateTime.UtcNow;
                await UpdateVolunteerHoursAsync(hours);
            }
        }

        public async Task RejectHoursAsync(int hoursId, int approverId, string reason)
        {
            var hours = await GetVolunteerHoursAsync(hoursId);
            if (hours != null)
            {
                hours.State = VolunteerState.Rejected;
                hours.ApproverId = approverId;
                hours.RejectionReason = reason;
                hours.ApprovedAt = DateTime.UtcNow;
                await UpdateVolunteerHoursAsync(hours);
            }
        }

        public async Task<List<VolunteerHours>> GetPendingHoursAsync()
        {
            return await _context.VolunteerHours
                .Include(vh => vh.VolunteerRequest)
                    .ThenInclude(vr => vr.Volunteer)
                .Where(vh => vh.State == VolunteerState.Pending )
                .OrderBy(vh => vh.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<VolunteerHours>> GetHoursByStateAsync(VolunteerState state)
        {
            return await _context.VolunteerHours
                .Include(vh => vh.VolunteerRequest)
                    .ThenInclude(vr => vr.Volunteer)
                .Include(vh => vh.Approver)
                .Where(vh => vh.State == state )
                .OrderByDescending(vh => vh.CreatedAt)
                .ToListAsync();
        }

        // ===== REPORTES Y ESTADÍSTICAS =====
        public async Task<decimal> GetTotalApprovedHoursAsync(int requestId)
        {
            return await _context.VolunteerHours
                .Where(vh => vh.VolunteerRequestId == requestId &&
                            vh.State == VolunteerState.Approved)
                .SumAsync(vh => vh.TotalHours);
        }

        public async Task<decimal> GetTotalWorkedHoursAsync(int requestId)
        {
            return await _context.VolunteerHours
                .Where(vh => vh.VolunteerRequestId == requestId &&                           
                            (vh.State == VolunteerState.Approved || vh.State == VolunteerState.Pending))
                .SumAsync(vh => vh.TotalHours);
        }

        public async Task<List<VolunteerHours>> GetHoursForReportAsync(int requestId, DateTime startDate, DateTime endDate)
        {
            return await _context.VolunteerHours
                .Where(vh => vh.VolunteerRequestId == requestId &&
                            vh.Date >= startDate &&
                            vh.Date <= endDate &&
                            vh.State == VolunteerState.Approved)
                .OrderBy(vh => vh.Date)
                .ToListAsync();
        }

        public async Task<int> GetTotalWorkDaysAsync(int requestId)
        {
            return await _context.VolunteerHours
                .Where(vh => vh.VolunteerRequestId == requestId &&
                            vh.State == VolunteerState.Approved)
                .Select(vh => vh.Date.Date)
                .Distinct()
                .CountAsync();
        }

        public async Task<DateTime?> GetLastWorkDateAsync(int requestId)
        {
            return await _context.VolunteerHours
                .Where(vh => vh.VolunteerRequestId == requestId &&
                            vh.State == VolunteerState.Approved)
                .MaxAsync(vh => (DateTime?)vh.Date);
        }

    }
}

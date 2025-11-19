using Api.Database.Entities;
using Shared.Enums;

namespace Api.Abstractions.Repositories
{
    public interface IVolunteerRequestRepository
    {
        Task CreateRequest(VolunteerRequest voluteerRequest);
        Task<VolunteerRequest> GetActiveRequest(int VolunteerId);
        Task<List<VolunteerRequest>> GetRequestsByVolunteerID(int volunteerID);

        // Gestión de solicitudes por administrador
        Task<List<VolunteerRequest>> GetAllRequestsAsync();
        Task<List<VolunteerRequest>> GetRequestsByStateAsync(VolunteerState state);
        Task<VolunteerRequest?> GetRequestByIdAsync(int requestId);
        Task UpdateRequestAsync(VolunteerRequest request);
        Task ApproveRequestAsync(int requestId, int approverId);
        Task RejectRequestAsync(int requestId, int approverId, string reason);

        // ===== GESTIÓN DE HORAS =====

        // CRUD básico de horas
        Task AddVolunteerHoursAsync(VolunteerHours hours);
        Task<VolunteerHours?> GetVolunteerHoursAsync(int hoursId);
        Task<List<VolunteerHours>> GetHoursByRequestIdAsync(int requestId);
        Task<List<VolunteerHours>> GetHoursByDateRangeAsync(int requestId, DateTime startDate, DateTime endDate);
        Task UpdateVolunteerHoursAsync(VolunteerHours hours);
        Task DeleteVolunteerHoursAsync(int hoursId);

        // Validaciones y restricciones
        Task<VolunteerHours?> GetHoursForDateAsync(int requestId, DateTime date);
        Task<decimal> GetTotalHoursForDateAsync(int requestId, DateTime date);
        Task<bool> HasHoursForDateAsync(int requestId, DateTime date);

        // Aprobación de horas
        Task ApproveHoursAsync(int hoursId, int approverId);
        Task RejectHoursAsync(int hoursId, int approverId, string reason);
        Task<List<VolunteerHours>> GetPendingHoursAsync();
        Task<List<VolunteerHours>> GetHoursByStateAsync(VolunteerState state);

        // Reportes y estadísticas
        Task<decimal> GetTotalApprovedHoursAsync(int requestId);
        Task<decimal> GetTotalWorkedHoursAsync(int requestId);
        Task<List<VolunteerHours>> GetHoursForReportAsync(int requestId, DateTime startDate, DateTime endDate);
        Task<int> GetTotalWorkDaysAsync(int requestId);
        Task<DateTime?> GetLastWorkDateAsync(int requestId);
    }
}


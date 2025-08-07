using Shared.Dtos.Volunteer;
using Shared.Enums;
using Shared.Models;

namespace Api.Abstractions.Application
{
    public interface IVolunteerRequestService
    {
        // ===== GESTIÓN DE SOLICITUDES =====
        Task<Result> CreateAsync(CreateVolunteerRequestDto requestDto, int volunteerId);
        Task<Result> CanCreateNewRequestAsync(int volunteerId); // ← metodo para validar si el voluntario puede crear una nueva solicitud
        Task<List<VolunteerRequestDto>> GetAllByVolunteerIDAsync(int volunteerId);

        // ===== MÉTODOS PARA ADMINISTRACIÓN =====
        Task<List<VolunteerRequestDto>> GetAllRequestsAsync();
        Task<List<VolunteerRequestDto>> GetRequestsByStateAsync(VolunteerState state);
        Task<Result<VolunteerRequestDto>> GetRequestByIdAsync(int requestId);
        Task<Result> ApproveRequestAsync(int requestId, int approverId);
        Task<Result> RejectRequestAsync(int requestId, int approverId, string reason);

        // ===== GESTIÓN DE HORAS =====
        Task<Result> CreateVolunteerHoursAsync(CreateVolunteerHoursDto dto);
        Task<Result<List<VolunteerHoursDto>>> GetHoursByRequestIdAsync(int requestId);
        Task<Result<List<VolunteerHoursDto>>> GetHoursByDateRangeAsync(int requestId, DateTime startDate, DateTime endDate);
        Task<Result<VolunteerHoursDto>> GetVolunteerHoursByIdAsync(int hoursId); // ✅ MÉTODO AGREGADO
        Task<Result> UpdateVolunteerHoursAsync(int hoursId, CreateVolunteerHoursDto dto);
        Task<Result> DeleteVolunteerHoursAsync(int hoursId);

        // ===== APROBACIÓN DE HORAS =====
        Task<Result> ApproveHoursAsync(ApproveRejectHoursDto dto);
        Task<Result> RejectHoursAsync(ApproveRejectHoursDto dto);
        Task<List<VolunteerHoursDto>> GetPendingHoursAsync();

        // ===== VALIDACIONES =====
        Task<Result> ValidateHoursAsync(CreateVolunteerHoursDto dto);
        Task<bool> CanRegisterHoursAsync(int requestId, DateTime date);
    }
}
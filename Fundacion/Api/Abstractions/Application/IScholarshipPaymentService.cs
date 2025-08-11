using Shared.Dtos.Financial;
using Shared.Models;

namespace Api.Abstractions.Application
{
    public interface IScholarshipPaymentService
    {
        Task<List<ScholarshipWithPaymentStatusDto>> GetScholarshipsWithPaymentStatusAsync();
        Task<Result<string>> ProcessPendingScholarshipsAsync(int userId);
        Task<Result> ProcessScholarshipPaymentAsync(int scholarshipId, int userId);
        Task<Result> SetScholarshipActiveStatusAsync(int scholarshipId, bool isActive);
    }
}
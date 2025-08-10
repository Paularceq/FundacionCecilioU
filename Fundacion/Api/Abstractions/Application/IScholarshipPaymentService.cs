using Shared.Models;

namespace Api.Abstractions.Application
{
    public interface IScholarshipPaymentService
    {
        Task<Result<string>> ProcessPendingScholarshipsAsync(int userId);
    }
}
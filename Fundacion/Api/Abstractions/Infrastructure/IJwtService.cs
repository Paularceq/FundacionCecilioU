using Shared.Models;

namespace Api.Abstractions.Infrastructure
{
    public interface IJwtService
    {
        string GenerateAccessToken(int userId, string userName, string userEmail, IEnumerable<string> roles);
        string GenerateForgotPasswordToken(int userId);
        Result<int> ValidateVerificationToken(string token);
    }
}
using Shared.Dtos;
using Shared.Models;

namespace Api.Abstractions.Application
{
    public interface IAuthService
    {
        Task<Result> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<Result<LoginResponseDto>> LoginAsync(LoginDto loginDto);
        Task<Result> RegisterAsync(RegisterDto registerDto);
        Task<Result> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    }
}
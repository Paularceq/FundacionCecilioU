using Shared.Dtos;
using Shared.Models;

namespace Api.Abstractions.Application
{
    public interface IUserProfileService
    {
        Task<Result> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
        Task<Result<UserProfileDto>> GetUserProfileAsync(int userId);
        Task<Result> UpdateUserProfileAsync(UserProfileUpdateDto updateDto);
    }
}
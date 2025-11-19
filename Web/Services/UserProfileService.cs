using Shared.Dtos;
using Shared.Models;
using Web.Http;

namespace Web.Services
{
    public class UserProfileService
    {
        private readonly ApiClient _apiClient;
        public UserProfileService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }
        public async Task<Result<UserProfileDto>> GetUserProfileAsync(int userId)
        {
            var result = await _apiClient.GetAsync<UserProfileDto>($"UserProfile/{userId}");
            if (result.IsFailure)
                return Result<UserProfileDto>.Failure(result.Errors);
            return Result<UserProfileDto>.Success(result.Value);
        }
        public async Task<Result> UpdateUserProfileAsync(UserProfileUpdateDto updateDto)
        {
            var response = await _apiClient.PutAsync("UserProfile", updateDto);
            if (response.IsFailure)
                return Result.Failure(response.Errors);
            return Result.Success();
        }
        public async Task<Result> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            var response = await _apiClient.PostAsync("UserProfile/ChangePassword", changePasswordDto);
            if (response.IsFailure)
                return Result.Failure(response.Errors);
            return Result.Success();

        }
    }
}

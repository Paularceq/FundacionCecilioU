using Shared.Dtos;
using Shared.Models;

namespace Api.Abstractions.Application
{
    public interface IUserManagementService
    {
        Task<Result> AddUserAsync(NewUserDto userDto);
        Task<Result> ChangeUserStatus(int id);
        Task<Result<IEnumerable<RoleDto>>> GetAllRoles();
        Task<Result<IEnumerable<UserToListDto>>> GetAllUsersAsync();
        Task<Result<UserDto>> GetUserByIdAsync(int id);
        Task<Result<IEnumerable<UserToListDto>>> GetUsersByRole(string roleName);
        Task<Result> UpdateUserAsync(UpdateUserDto userDto);
    }
}
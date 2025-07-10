using Shared.Dtos;
using Shared.Models;

namespace Api.Abstractions.Application
{
    public interface IUserManagementService
    {
        Task<Result> AddUserAsync(NewUserDto userDto);
        Task<Result<IEnumerable<RoleDto>>> GetAllRoles();
        Task<Result<IEnumerable<UsertoListDto>>> GetAllUsersAsync();
    }
}
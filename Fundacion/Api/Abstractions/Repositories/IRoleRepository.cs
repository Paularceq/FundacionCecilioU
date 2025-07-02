using Api.Database.Entities;

namespace Api.Abstractions.Repositories
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<Role> GetRoleByNameAsync(string roleName);
        Task<IEnumerable<Role>> GetRolesByIdsAsync(IEnumerable<int> ids);
        Task<IEnumerable<Role>> GetRolesByNamesAsync(IEnumerable<string> roleNames);
    }
}
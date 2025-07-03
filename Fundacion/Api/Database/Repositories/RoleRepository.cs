using Api.Abstractions.Repositories;
using Api.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Database.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly DatabaseContext _context;

        public RoleRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Role>> GetRolesByIdsAsync(IEnumerable<int> ids)
        {
            return await _context.Roles
                .Where(c => ids.Contains(c.Id))
                .ToListAsync();
        }

        public async Task<Role> GetRoleByNameAsync(string roleName)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(c => c.Name == roleName);
        }

        public async Task<IEnumerable<Role>> GetRolesByNamesAsync(IEnumerable<string> roleNames)
        {
            return await _context.Roles
                 .Where(c => roleNames.Contains(c.Name))
                 .ToListAsync();
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _context.Roles

                .ToListAsync();
        }

    }
}

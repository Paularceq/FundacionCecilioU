using Api.Abstractions.Repositories;
using Api.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Database.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseContext _context;

        public UserRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<User> GetUserByIdentificacionAsync(string identificacion)
        {
            return await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Identificacion == identificacion);
        }

        public async Task<User> AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }
        
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Roles)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByRole(string roleName)
        {
            return await _context.Users
                .Where(u => u.Activo && u.Roles.Any(r => r.Name == roleName))
                .ToListAsync();
        }

        public async Task<string> SetSessionIdAsync(User user)
        {
            user.SessionId = Guid.NewGuid();
            await _context.SaveChangesAsync();
            return user.SessionId.ToString();
        }
    }
}

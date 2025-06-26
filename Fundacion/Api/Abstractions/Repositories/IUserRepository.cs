using Api.Database.Entities;

namespace Api.Abstractions.Repositories
{
    public interface IUserRepository
    {
        Task<User> AddUserAsync(User user);
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> UpdateUserAsync(User user);
        Task<IEnumerable<User>> GetAllUsersAsync();
    }
}
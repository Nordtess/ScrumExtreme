using ScrumExtreme.Domain.Entities;

namespace ScrumExtreme.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task CreateUserAsync(User user);
    Task<User?> GetByIdAsync(string id);
    Task DeleteUserAsync(string id);
    Task<User?> GetByUsernameAsync(string username);
}

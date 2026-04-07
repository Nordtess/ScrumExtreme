using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;
using ScrumExtreme.Domain.Interfaces;

namespace ScrumExtreme.Application.Services;

public class UserService : IUserService
{
    private readonly IRepository<User> _repository;

    public UserService(IRepository<User> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync() =>
        await _repository.GetAllAsync();

    public async Task CreateUserAsync(User user) =>
        await _repository.AddAsync(user);

    public async Task<User?> GetByIdAsync(string id) =>
        await _repository.GetByIdAsync(id);
}

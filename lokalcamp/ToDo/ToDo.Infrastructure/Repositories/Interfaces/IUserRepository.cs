using ToDo.Domain.Models;

namespace ToDo.Data.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User> GetByUserIdAsync(Guid userId);
    Task<User> AddAsync(User user);
}
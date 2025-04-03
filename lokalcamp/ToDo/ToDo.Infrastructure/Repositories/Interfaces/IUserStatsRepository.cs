using ToDo.Domain.Models;

namespace ToDo.Data.Repositories.Interfaces
{
    public interface IUserStatsRepository
    {
        Task<UserStat> GetByUserIdAsync(Guid userId);
        Task UpdateAsync(UserStat userStat);
    }
}

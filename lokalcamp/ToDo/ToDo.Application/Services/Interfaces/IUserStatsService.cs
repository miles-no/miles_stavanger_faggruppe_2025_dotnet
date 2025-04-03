using ToDo.Business.Dtos;

namespace ToDo.Business.Services.Interfaces
{
    public interface IUserStatsService
    {
        Task<UserStatsDto?> GetByUserIdAsync(Guid id);
        Task<List<UserStatsDto>> GetLeaderboardAsync(int numberOfUsersToFetch);
    }
}

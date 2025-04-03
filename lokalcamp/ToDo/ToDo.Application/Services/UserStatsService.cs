using ToDo.Business.Dtos;
using ToDo.Business.Services.Interfaces;
using ToDo.Data.Repositories.Interfaces;

namespace ToDo.Business.Services
{
    public class UserStatsService(IUserStatsRepository userStatsRepository) : IUserStatsService
    {
        public async Task<UserStatsDto?> GetByUserIdAsync(Guid id)
        {
            var entity = await userStatsRepository.GetByUserIdAsync(id);
            return new UserStatsDto(entity);
        }

        public async Task<List<UserStatsDto>> GetLeaderboardAsync(int numberOfUsersToFetch)
        {
            var topUsers = await userStatsRepository.GetTopUsersByPointsAsync(numberOfUsersToFetch);
            return topUsers.Select(u => new UserStatsDto(u)).ToList();
        }
    }
}

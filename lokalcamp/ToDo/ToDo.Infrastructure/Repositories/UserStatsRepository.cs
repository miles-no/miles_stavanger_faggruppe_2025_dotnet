using Microsoft.EntityFrameworkCore;
using ToDo.Data.Repositories.Interfaces;
using ToDo.Domain.Models;

namespace ToDo.Data.Repositories
{
    public class UserStatsRepository(ApplicationDbContext context) : IUserStatsRepository
    {
        public async Task<UserStat> GetByUserIdAsync(Guid userId)
            => await context.UserStats.FirstAsync(s => s.UserId == userId);

        public async Task UpdateAsync(UserStat userStat)
        {
            context.UserStats.Update(userStat);
            await context.SaveChangesAsync();
        }
    }
}

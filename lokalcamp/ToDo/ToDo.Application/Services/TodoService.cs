using ToDo.Business.Dtos;
using ToDo.Business.Services.Interfaces;
using ToDo.Data.Repositories.Interfaces;
using ToDo.Domain.Models;

namespace ToDo.Business.Services
{
    public class TodoService(ITodoRepository todoRepository, IUserStatsRepository userStatsRepository) : ITodoService
    {
        public async Task<List<TodoDto>> GetAllAsync()
        {
            return (await todoRepository.GetAllAsync()).Select(todo => new TodoDto(todo)).ToList();
        }

        public async Task<TodoDto> AddAsync(TodoDto todo)
        {
            var entity = new TodoItem { Title = todo.Title, Completed = todo.Completed, UserId = Domain.Constants.DefaultUserId };
            await todoRepository.AddAsync(entity);
            return new TodoDto(entity);
        }

        public async Task<TodoDto?> GetByIdAsync(Guid id)
        {
            var entity = await todoRepository.GetAsync(id);
            return entity != null ? new TodoDto(entity) : null;
        }

        public async Task<TodoDto> UpdateAsync(Guid id, TodoDto todo)
        {
            var entity = await todoRepository.GetAsync(id);
            if (entity == null) throw new Exception("Task not found");

            await UpdateUserStatsOnCompletedTask(todo, entity);

            entity.Completed = todo.Completed;
            entity.Title = todo.Title;
            await todoRepository.UpdateAsync(entity);

            return new TodoDto(entity);
        }

        private async Task UpdateUserStatsOnCompletedTask(TodoDto todo, TodoItem entity)
        {
            if (!(todo.Completed && !entity.Completed))
            {
                return;
            }

            entity.CompletedAt = DateTime.UtcNow;

            // Update user stats
            var userId = Domain.Constants.DefaultUserId; // only one user atm ;)
            var userStats = await userStatsRepository.GetByUserIdAsync(userId);
            userStats.Points += 10; // Earn 10 points per task
            userStats.StreakDays = UpdateStreak(userStats);
            userStats.Level = CalculateLevel(userStats.Points);

            await userStatsRepository.UpdateAsync(userStats);
        }

        public void Delete(Guid id)
        {
            todoRepository.Delete(id);
        }

        private int CalculateLevel(int points)
        {
            return (points / 50) + 1; // Level up every 50 points
        }

        private int UpdateStreak(UserStat stat)
        {
            if (stat.StreakDays == 0 || (stat.StreakDays > 0 && stat.LastCompletedDate?.Date == DateTime.UtcNow.AddDays(-1).Date))
            {
                return stat.StreakDays + 1; // Continue streak
            }
            return 1; // Reset streak if a day is missed
        }
    }

}

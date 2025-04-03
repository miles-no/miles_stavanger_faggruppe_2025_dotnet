using ToDo.Domain.Models;

namespace ToDo.Business.Dtos
{
    public class UserStatsDto
    {
        public UserStatsDto(UserStat entity)
        {
            UserId = entity.UserId;
            StreakDays = entity.StreakDays;
            Points = entity.Points;
            Level = entity.Level;
        }

        public Guid UserId { get; set; }
        public int Points { get; set; }
        public int Level { get; set; }
        public int StreakDays { get; set; }
    }
}

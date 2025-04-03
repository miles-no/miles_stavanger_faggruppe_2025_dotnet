using ToDo.Domain.Models;

namespace ToDo.Business.Dtos
{
    public class UserStatsDto
    {
        public UserStatsDto(UserStat entity)
        {
            StreakDays = entity.StreakDays;
            Points = entity.Points;
            Level = entity.Level;
        }

        public int Points { get; set; }
        public int Level { get; set; }
        public int StreakDays { get; set; }
    }
}

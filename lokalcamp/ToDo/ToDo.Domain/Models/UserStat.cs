namespace ToDo.Domain.Models
{
    public class UserStat
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public int Points { get; set; } = 0;
        public int Level { get; set; } = 1;
        public int StreakDays { get; set; } = 0;
        public DateTime? LastCompletedDate { get; set; }
    }
}

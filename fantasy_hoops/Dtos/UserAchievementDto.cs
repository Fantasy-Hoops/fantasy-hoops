using System;

namespace fantasy_hoops.Dtos
{
    public class UserAchievementDto
    {
        public String UserId { get; set; }
        public String UserName { get; set; }
        public double Progress { get; set; }
        public int Level { get; set; }
        public int LevelUpGoal { get; set; }
        public bool IsAchieved { get; set; }
        public AchievementDto Achievement { get; set; }
    }
}
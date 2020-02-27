using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace fantasy_hoops.Models.Achievements
{
    public class UserAchievement
    {
        [DefaultValue(0)]
        public double Progress { get; set; }
        [DefaultValue(1)]
        public int Level { get; set; }
        public int LevelUpGoal { get; set; }
        
        [DefaultValue(0)]
        public bool IsAchieved { get; set; }

        [Key]
        public string UserID { get; set; }
        public User User { get; set; }
        [Key]
        public int AchievementID { get; set; }
        public Achievement Achievement { get; set; }
    }
}
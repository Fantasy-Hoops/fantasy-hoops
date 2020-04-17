using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [DatabaseGenerated(DatabaseGeneratedOption.None), Key]
        public string UserID { get; set; }
        public virtual User User { get; set; }
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None), Key]
        public int AchievementID { get; set; }
        public virtual Achievement Achievement { get; set; }
    }
}
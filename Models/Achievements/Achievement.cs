using System;
using System.ComponentModel.DataAnnotations;

namespace fantasy_hoops.Models.Achievements
{
    public class Achievement
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public AchievementType Type { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public string CompletedMessage { get; set; }
        [Required]
        public string Icon { get; set; }
        public int GoalBase { get; set; }
        
        public enum AchievementType
        {
            SINGLE_LEVEL, MULTI_LEVEL
        }
    }
}
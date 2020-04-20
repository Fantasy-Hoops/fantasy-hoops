using fantasy_hoops.Models.Enums;

namespace fantasy_hoops.Dtos
{
    public class AchievementDto
    {
        public int Id { get; set; }
        public AchievementType Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CompletedMessage { get; set; }
        public string Icon { get; set; }
        public int GoalBase { get; set; }
    }
}
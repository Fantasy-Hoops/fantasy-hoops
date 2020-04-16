using System.ComponentModel.DataAnnotations;

namespace fantasy_hoops.Models.Tournaments
{
    public class MatchupPair
    {
        [Key]
        public string TournamentID { get; set; }
        [Key]
        public string FirstUserID { get; set; }
        [Key]
        public string SecondUserID { get; set; }
        [Key]
        public int ContestId { get; set; }
        public double FirstUserScore { get; set; }
        public double SecondUserScore { get; set; }
        
        public virtual Tournament Tournament { get; set; }
        public virtual User FirstUser { get; set; }
        public virtual User SecondUser { get; set; }
    }
}
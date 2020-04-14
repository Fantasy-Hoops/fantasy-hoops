using System.ComponentModel.DataAnnotations;

namespace fantasy_hoops.Models.Tournaments
{
    public class TournamentUser
    {
        [Key]
        public string UserID { get; set; }
        [Key]
        public string TournamentID { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Ties { get; set; }
        public bool IsEliminated { get; set; }
        
        public virtual User User { get; set; }
        public virtual Tournament Tournament { get; set; }
    }
}
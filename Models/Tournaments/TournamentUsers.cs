using System.ComponentModel.DataAnnotations;

namespace fantasy_hoops.Models.Tournaments
{
    public class TournamentUsers
    {
        [Key]
        public string UserID { get; set; }
        [Key]
        public int TournamentID { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Ties { get; set; }
        
        public virtual User User { get; set; }
        public virtual Tournament Tournament { get; set; }
    }
}
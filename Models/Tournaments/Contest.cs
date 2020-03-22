using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace fantasy_hoops.Models.Tournaments
{
    public class Contest
    {
        [Key]
        public int Id { get; set; }
        public DateTime ContestStart { get; set; }
        public List<MatchupPair> ContestPairs { get; set; }
        
        
        public string TournamentID { get; set; }
        public virtual Tournament Tournament { get; set; }
    }
}
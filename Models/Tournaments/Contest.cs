using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace fantasy_hoops.Models.Tournaments
{
    public class Contest
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("TournamentId")]
        public string TournamentId { get; set; }
        public int ContestNumber { get; set; }
        public DateTime ContestStart { get; set; }
        public DateTime ContestEnd { get; set; }
        public bool IsFinished { get; set; }
        public string WinnerId { get; set; }
        public User Winner { get; set; }
        public List<MatchupPair> Matchups { get; set; }
    }
}
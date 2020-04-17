using System;
using System.Collections.Generic;

namespace fantasy_hoops.Dtos
{
    public class ContestDto
    {
        public int Id { get; set; }
        public int ContestNumber { get; set; }
        public string TournamentId { get; set; }
        public DateTime ContestStart { get; set; }
        public DateTime ContestEnd { get; set; }
        public TournamentUserDto Winner { get; set; }
        public bool IsFinished { get; set; }
        public List<MatchupPairDto> Matchups { get; set; }
    }
}
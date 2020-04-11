using System;
using System.Collections.Generic;
using fantasy_hoops.Models;

namespace fantasy_hoops.Dtos
{
    public class ContestDto
    {
        public DateTime ContestStart { get; set; }
        public DateTime ContestEnd { get; set; }
        public UserDto Winner { get; set; }
        public bool IsFinished { get; set; }
        public List<MatchupPairDto> Matchups { get; set; }
    }
}
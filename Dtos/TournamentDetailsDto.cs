using System.Collections.Generic;

namespace fantasy_hoops.Dtos
{
    public class TournamentDetailsDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public bool IsCreator { get; set; }
        public List<ContestDto> Contests { get; set; }
        public List<TournamentUserDto> Standings { get; set; }
        public UserLeaderboardRecordDto CurrentLineup { get; set; }
        public string NextOpponent { get; set; }
    }
}
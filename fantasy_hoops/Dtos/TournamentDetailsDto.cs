using System;
using System.Collections.Generic;
using fantasy_hoops.Enums;
using fantasy_hoops.Models;

namespace fantasy_hoops.Dtos
{
    public class TournamentDetailsDto
    {
        public string Id { get; set; }
        public TournamentStatus Status { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TypeName { get; set; }
        public string ImageURL { get; set; }
        public UserDto Creator { get; set; }
        public bool IsCreator { get; set; }
        public List<ContestDto> Contests { get; set; }
        public List<TournamentUserDto> Standings { get; set; }
        public UserLeaderboardRecordDto CurrentLineup { get; set; }
        public string NextOpponent { get; set; }
        public bool AcceptedInvite { get; set; }
    }
}
using System;
using System.Collections.Generic;

namespace fantasy_hoops.Dtos
{
    public class UserLeaderboardRecordDto
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime Date { get; set; }
        public string LongDate { get; set; }
        public string ShortDate { get; set; }
        public List<LineupPlayerDto> Lineup { get; set; }
        public double FP { get; set; }
        public int Price { get; set; }
        public int GamesPlayed { get; set; }
        public bool IsLive { get; set; }
    }
}

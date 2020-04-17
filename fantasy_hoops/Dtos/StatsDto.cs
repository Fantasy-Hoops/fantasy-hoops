using System.Collections.Generic;

namespace fantasy_hoops.Dtos
{
    public class StatsDto
    {
        public int PlayerID { get; set; }
        public int NbaID { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AbbrName { get; set; }
        public string Number { get; set; }
        public string Position { get; set; }
        public double PTS { get; set; }
        public double REB { get; set; }
        public double AST { get; set; }
        public double STL { get; set; }
        public double BLK { get; set; }
        public double TOV { get; set; }
        public double FPPG { get; set; }
        public int Price { get; set; }
        public TeamDto Team { get; set; }
        public List<GameStatsDto> Games { get; set; }
        public StatsPercentagesDto Percentages { get; set; }
    }
}

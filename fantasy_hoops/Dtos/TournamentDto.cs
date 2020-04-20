using System;
using System.Collections.Generic;
using fantasy_hoops.Models.Enums;
using fantasy_hoops.Models.Tournaments;

namespace fantasy_hoops.Dtos
{
    public class TournamentDto
    {
        public string Id { get; set; }
        public TournamentStatus Status { get; set; }
        public int Type { get; set; }
        public string TypeName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
        public int Entrants { get; set; }
        public List<Contest> Contests { get; set; }
        public int DroppedContests { get; set; }
    }
}
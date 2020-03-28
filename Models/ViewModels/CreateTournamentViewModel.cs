using System;
using System.Collections.Generic;

namespace fantasy_hoops.Models.ViewModels
{
    public class CreateTournamentViewModel
    {
        public string CreatorId { get; set; }
        public string TournamentIcon { get; set; }
        public string TournamentTitle { get; set; }
        public string TournamentDescription { get; set; }
        public DateTime StartDate { get; set; }
        public int TournamentType { get; set; }
        public int Contests { get; set; }
        public int DroppedContests { get; set; }
        public int Entrants { get; set; }
        public List<String> UserFriends { get; set; }
    }
}
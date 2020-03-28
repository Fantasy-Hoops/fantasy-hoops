using System;
using System.ComponentModel.DataAnnotations;

namespace fantasy_hoops.Models.Tournaments
{
    public class Tournament
    {
        [Key] public string Id { get; set; }
        public int TypeID { get; set; }
        public TournamentType Type { get; set; }
        public string CreatorID { get; set; }
        public User Creator { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
        public int Entrants { get; set; }
        [Range(0, Int32.MaxValue)] public int Contests { get; set; }
        [Range(0, 50)] public int DroppedContests { get; set; }

        public class TournamentType
        {
            [Key] public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
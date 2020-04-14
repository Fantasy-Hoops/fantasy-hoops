using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace fantasy_hoops.Models.Tournaments
{
    public class Tournament
    {
        [Key] public string Id { get; set; }
        public int Type { get; set; }
        public bool IsActive { get; set; }
        public bool IsFinished { get; set; }
        public string CreatorID { get; set; }
        public User Creator { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
        public int Entrants { get; set; }
        [Range(0, 50)] public int DroppedContests { get; set; }
        public virtual List<Contest> Contests { get; set; }
        public virtual List<TournamentInvite> Invites { get; set; }
        public string WinnerID { get; set; }
        public virtual User Winner { get; set; }

        public sealed class TournamentType
        {
            private static readonly Dictionary<int, TournamentType> instance =
                new Dictionary<int, TournamentType>();

            private readonly int id;
            private readonly String name;

            public static readonly TournamentType ONE_FOR_ALL = new TournamentType(0, "One For All");
            public static readonly TournamentType MATCHUPS = new TournamentType(1, "Matchups");

            private TournamentType(int id, String name)
            {
                this.id = id;
                this.name = name;
                instance[id] = this;
            }

            public static explicit operator TournamentType(int tournamentType)
            {
                if (instance.TryGetValue(tournamentType, out var result))
                    return result;
                throw new InvalidCastException();
            }

            public static List<TournamentType> Values()
            {
                return new List<TournamentType> {ONE_FOR_ALL, MATCHUPS};
            }

            public int GetId()
            {
                return id;
            }

            public override String ToString()
            {
                return name;
            }
        }
    }
}
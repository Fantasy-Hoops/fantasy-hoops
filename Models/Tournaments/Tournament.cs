using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using fantasy_hoops.Enums;

namespace fantasy_hoops.Models.Tournaments
{
    public class Tournament
    {
        [Key] public string Id { get; set; }
        public int Type { get; set; }
        [Required, Range(0, 1)] public TournamentStatus Status { get; set; }
        [Required] public string CreatorID { get; set; }
        public User Creator { get; set; }
        [Required] public DateTime StartDate { get; set; }
        [Required] public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Tournament title is required"),
         StringLength(maximumLength: 20, MinimumLength = 5,
            ErrorMessage = "Tournament title length must be between 5-20 characters long")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Tournament description is required"),
         StringLength(maximumLength: 50, MinimumLength = 5,
            ErrorMessage = "Tournament title length must be between 5-20 characters long")]
        public string Description { get; set; }

        [Required] public string ImageURL { get; set; }
        
        [Range(0, 50)] public int Entrants { get; set; }
        public int DroppedContests { get; set; }
        public List<Contest> Contests { get; set; }
        public List<TournamentInvite> Invites { get; set; }
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
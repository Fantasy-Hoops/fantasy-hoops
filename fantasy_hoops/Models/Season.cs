using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace fantasy_hoops.Models
{
    public class Season
    {
        [Key]
        public string Id { get; set; }
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PreSeasonGames { get; set; }
        public int RegularSeasonGames { get; set; }
        public int AllStarBreakGames { get; set; }
        public int PlayoffGames { get; set; }
        public virtual IList<Game> Games { get; set; }
    }
}
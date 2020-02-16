using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace fantasy_hoops.Models
{
    public class Team
    {
        [Key]
        public int TeamID { get; set; }
        public int NbaID { get; set; }
        public String Name { get; set; }
        public String City { get; set; }
        public String Abbreviation { get; set; }
        public String Color { get; set; }
        public int? NextOpponentID { get; set; }
        public String NextOppFormatted { get; set; }

        public virtual Team NextOpponent { get; set; }
        public virtual ICollection<Player> Players { get; set; }
    }
}

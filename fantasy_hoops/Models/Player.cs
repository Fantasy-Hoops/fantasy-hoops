﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace fantasy_hoops.Models
{
    public class Player
    {
        [Key]
        public int PlayerID { get; set; }
        public int NbaID { get; set; }
        public String FullName { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String AbbrName { get; set; }
        public String Position { get; set; }
        public string Number { get; set; }
        public int GP { get; set; }
        public double PTS { get; set; }
        public double REB { get; set; }
        public double AST { get; set; }
        public double STL { get; set; }
        public double BLK { get; set; }
        public double TOV { get; set; }
        public double FPPG { get; set; }
        public int Price { get; set; }
        public int PreviousPrice { get; set; }
        public int TeamID { get; set; }
        public bool IsPlaying { get; set; }
        public bool IsInGLeague { get; set; }

        public virtual Team Team { get; set; }
        public int InjuryID { get; set; }
        public virtual Injury Injury { get; set; }
        public virtual ICollection<Stats> Stats { get; set; }
        public virtual IEnumerable<PlayersBestLineups> BestLineups { get; set; }
    }
}

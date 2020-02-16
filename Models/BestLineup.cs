using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace fantasy_hoops.Models
{
    public class BestLineup
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "Date")]
        public DateTime Date { get; set; }
        public IEnumerable<PlayersBestLineups> Lineup { get; set; }
        public double TotalFP { get; set; }
        public int LineupPrice { get; set; }
    }
}
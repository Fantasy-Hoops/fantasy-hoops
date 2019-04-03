using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace fantasy_hoops.Models
{
    public class Game
    {

        [Key]
        public int GameID { get; set; }

        public DateTime? Date { get; set; }

        public int HomeTeamID { get; set; }

        public int HomeScore { get; set; }

        public int AwayTeamID { get; set; }

        public int AwayScore { get; set; }

        [ForeignKey("HomeTeamID")]
        public virtual Team HomeTeam { get; set; }

        [ForeignKey("AwayTeamID")]
        public virtual Team AwayTeam { get; set; }


    }
}

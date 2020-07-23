using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using fantasy_hoops.Models.Enums;

namespace fantasy_hoops.Models
{
    public class Game
    {
        [Key]
        public int Id { get; set; }
        public string Reference { get; set; }
        public SeasonStage SeasonStage { get; set; }
        public GameStatus Status { get; set; }
        public bool isTBD { get; set; }
        public DateTime? Date { get; set; }
        public int HomeTeamID { get; set; }
        public int HomeScore { get; set; }
        public int AwayTeamID { get; set; }
        public int AwayScore { get; set; }
        public string SeasonId { get; set; }

        [ForeignKey("HomeTeamID")]
        public virtual Team HomeTeam { get; set; }
        [ForeignKey("AwayTeamID")]
        public virtual Team AwayTeam { get; set; }
        
        [ForeignKey("SeasonId")]
        public virtual Season Season { get; set; }
    }
}

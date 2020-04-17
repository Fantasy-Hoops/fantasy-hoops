using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using fantasy_hoops.Enums;

namespace fantasy_hoops.Models
{
    public class News
    {
        [Key]
        public int NewsID { get; set; }

        public NewsType Type { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        
        public int hTeamID { get; set; }
        public int vTeamID { get; set; }
        public virtual ICollection<Paragraph> Paragraphs { get; set; }
        
        [ForeignKey("hTeamID")]
        public virtual Team hTeam { get; set; }
        [ForeignKey("vTeamID")]
        public virtual Team vTeam { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace fantasy_hoops.Models
{
    public enum NewsType
    {
        PREVIEW, RECAP
    }
    
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
        public Team hTeam { get; set; }
        [ForeignKey("vTeamID")]
        public Team vTeam { get; set; }
    }
}

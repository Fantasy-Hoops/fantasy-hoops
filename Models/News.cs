using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace fantasy_hoops.Models
{
    public class News
    {
        [Key]
        public int NewsID { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public int hTeamID { get; set; }
        public int vTeamID { get; set; }

        public virtual ICollection<Paragraph> Paragraphs { get; set; }
    }
}

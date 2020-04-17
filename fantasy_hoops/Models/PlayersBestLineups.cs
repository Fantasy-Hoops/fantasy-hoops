using System.ComponentModel.DataAnnotations;

namespace fantasy_hoops.Models
{
    public class PlayersBestLineups
    {
        [Key]
        public int PlayerID { get; set; }
        [Key]
        public int BestLineupID { get; set; }
        public double FP { get; set; }
        public int Price { get; set; }
        
        public virtual Player Player { get; set; }
        public virtual BestLineup BestLineup { get; set; }
    }
}
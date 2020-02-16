using fantasy_hoops.Models;

namespace fantasy_hoops.Dtos
{
    public class LineupPlayerDto
    {
        public Player Player { get; set; }
        public double FP { get; set; }
        public int Price { get; set; }
    }
}
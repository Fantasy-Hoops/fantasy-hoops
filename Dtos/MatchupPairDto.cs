namespace fantasy_hoops.Dtos
{
    public class MatchupPairDto
    {
        public UserDto FirstUser { get; set; }
        public double FirstUserScore { get; set; }
        public UserDto SecondUser { get; set; }
        public double SecondUserScore { get; set; }
    }
}
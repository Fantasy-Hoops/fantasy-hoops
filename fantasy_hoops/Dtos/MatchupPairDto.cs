namespace fantasy_hoops.Dtos
{
    public class MatchupPairDto
    {
        public TournamentUserDto FirstUser { get; set; }
        public double FirstUserScore { get; set; }
        public TournamentUserDto SecondUser { get; set; }
        public double SecondUserScore { get; set; }
    }
}
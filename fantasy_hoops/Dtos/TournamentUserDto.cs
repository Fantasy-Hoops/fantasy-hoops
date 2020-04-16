namespace fantasy_hoops.Dtos
{
    public class TournamentUserDto : UserDto
    {
        public string TournamentId { get; set; }
        public int Position { get; set; }
        public int W { get; set; }
        public int L { get; set; }
        public int Points { get; set; }
    }
}
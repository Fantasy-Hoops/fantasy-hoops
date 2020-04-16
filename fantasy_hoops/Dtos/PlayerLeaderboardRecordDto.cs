namespace fantasy_hoops.Dtos
{
    public class PlayerLeaderboardRecordDto
    {
        public int NbaId { get; set; }
        public string TeamColor { get; set; }
        public string FullName { get; set; }
        public string AbbrName { get; set; }
        public double FP { get; set; }
        public string LongDate { get; set; }
        public string ShortDate { get; set; }
        public int Count { get; set; }
    }
}
using System;

namespace fantasy_hoops.Dtos
{
    public class GameStatsDto
    {
        public int StatsID { get; set; }
        public DateTime Date { get; set; }
        public OpponentDto Opponent { get; set; }
        public string Score { get; set; }
        public string MIN { get; set; }
        public int FGM { get; set; }
        public int FGA { get; set; }
        public string FGP { get; set; }
        public int TPM { get; set; }
        public int TPA { get; set; }
        public string TPP { get; set; }
        public int FTM { get; set; }
        public int FTA { get; set; }
        public string FTP { get; set; }
        public int DREB { get; set; }
        public int OREB { get; set; }
        public int TREB { get; set; }
        public int AST { get; set; }
        public int BLK { get; set; }
        public int STL { get; set; }
        public int FLS { get; set; }
        public int TOV { get; set; }
        public int PTS { get; set; }
        public double GS { get; set; }
        public double FP { get; set; }
        public int Price { get; set; }
    }
}

using System;

namespace fantasy_hoops.Dtos
{
    public class NotificationDto
    {
        public int NotificationID { get; set; }
        public string ReceiverID { get; set; }
        public DateTime DateCreated { get; set; }
        public bool ReadStatus { get; set; }
        
        // Game score notification info
        public double? Score { get; set; }
        
        // Friend notification info
        public string SenderID { get; set; }
        public string RequestMessage { get; set; }
        public string FriendUsername { get; set; }
        public string FriendAvatarUrl { get; set; }
        
        // If tournament request
        public string TournamentId { get; set; }
        
        // Injury notification info
        public int? PlayerID { get; set; }
        public int? NbaId { get; set; }
        public string AbbrName { get; set; }
        public string Position { get; set; }
        public string TeamColor { get; set; }
        public string InjuryDescription { get; set; }
        public string InjuryStatus { get; set; }
    }
}
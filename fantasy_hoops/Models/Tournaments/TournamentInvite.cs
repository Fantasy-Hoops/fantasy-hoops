using System.ComponentModel.DataAnnotations;
using fantasy_hoops.Models.Enums;

namespace fantasy_hoops.Models.Tournaments
{
    public class TournamentInvite
    {
        [Key] public int Id { get; set; }
        public string InvitedUserID { get; set; }
        public string TournamentID { get; set; }
        public RequestStatus Status { get; set; }
        
        public virtual User InvitedUser { get; set; }
        public virtual Tournament Tournament { get; set; }
    }
}
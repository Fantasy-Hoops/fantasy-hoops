using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using fantasy_hoops.Models.Achievements;

namespace fantasy_hoops.Models
{
    public class User : IdentityUser
    {
        public string AvatarURL { get; set; }
        public String Description { get; set; }
        public int Streak { get; set; }
        public bool IsSocialAccount { get; set; }

        
        public int FavoriteTeamId { get; set; }
        public virtual Team FavoriteTeam { get; set; }
        public virtual ICollection<UserLineup> UserLineups { get; set; }
        public virtual ICollection<UserAchievement> Achievements { get; set; }
    }
}

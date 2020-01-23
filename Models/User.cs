using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace fantasy_hoops.Models
{
    public class User : IdentityUser
    {
        public string AvatarURL { get; set; }
        public String Description { get; set; }
        public int FavoriteTeamId { get; set; }
        public virtual Team Team { get; set; }
        public int Streak { get; set; }
        public bool IsSocialAccount { get; set; } = false;

        public virtual ICollection<UserLineup> UserLineups { get; set; }
    }
}

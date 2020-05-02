using System;
using System.Collections.Generic;

namespace fantasy_hoops.Models.ViewModels
{
    public class UpdateUserRolesViewModel
    {
        public String UserId { get; set; }
        public UserRolesViewModel UserRoles { get; set; }
    }
}
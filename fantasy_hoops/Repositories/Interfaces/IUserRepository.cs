﻿using System;
using System.Collections.Generic;
using System.Linq;
using fantasy_hoops.Dtos;
using fantasy_hoops.Models;
using Microsoft.AspNetCore.Identity;

namespace fantasy_hoops.Repositories.Interfaces
{
    public interface IUserRepository
    {

        object GetProfile(string id, int start = 0, int count = 0);
        User GetUserById(string id);
        User GetUserByName(string username);
        User GetUserByEmail(string email);
        IQueryable<Object> GetFriends(string id);
        IQueryable<Object> GetUserPool();
        List<UserDto> GetAllUsers();
        bool UserExists(string username);
        bool EmailExists(string email);
        IQueryable<Object> GetTeam(string id);
        bool IsDuplicateUserName(string id, string username);
        bool IsDuplicateEmail(string id, string email);
        string GetAdminRoleId();
        List<IdentityUserRole<string>> GetAdmins(string adminRoleId);
        IQueryable<object> Roles(string id);
        bool IsAdmin(string userId);
        void DeleteUserResources(User userToDelete);

    }
}

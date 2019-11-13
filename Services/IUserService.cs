using fantasy_hoops.Models;
using fantasy_hoops.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace fantasy_hoops.Services
{
    public interface IUserService
    {

        Task<bool> Login(LoginViewModel model);
        Task<bool> Register(RegisterViewModel model);
        Task<bool> GoogleRegister(ClaimsPrincipal model);
        void Logout();
        string RequestToken(string username);
        string RequestTokenById(string id);
        Task<string> RequestTokenByEmail(string id);
        Task<bool> UpdateProfile(EditProfileViewModel model);
        bool UploadAvatar(AvatarViewModel model);
        bool ClearAvatar(AvatarViewModel model);

    }
}

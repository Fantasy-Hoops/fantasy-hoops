using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models;
using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Repositories;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace fantasy_hoops.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration Configuration;
        private readonly IPushService _pushService;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserService(IConfiguration configuration, IPushService pushService, IUserRepository userRepository, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            Configuration = configuration;
            _pushService = pushService;
            _userRepository = userRepository;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<bool> Login(LoginViewModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, true, lockoutOnFailure: false);
            return result.Succeeded;
        }

        public async Task<bool> Register(RegisterViewModel model)
        {
            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await SendRegisterNotification(model.UserName);
            }

            return result.Succeeded;
        }

        public async void Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<string> RequestTokenByEmail(string email)
        {
            User user = await _userManager.FindByEmailAsync(email);
            return await RequestToken(user.UserName);
        }

        public async Task<string> RequestTokenById(string id)
        {
            string userName = _userRepository.GetUser(id).UserName;
            return await RequestToken(userName);
        }

        public async Task<string> RequestToken(string username)
        {
            User user = await _userManager.FindByNameAsync(username);
            bool isAdmin = _userRepository.IsAdmin(user.Id);
            IList<string> roles = await _userManager.GetRolesAsync(user);

            List<Claim> claims = new List<Claim>
            {
                new Claim("id", user.Id),
                new Claim("username", user.UserName),
                new Claim("email", user.Email),
                new Claim("description", user.Description ??""),
                new Claim("team", user.Team != null ? user.Team.Name : ""),
                new Claim("isAdmin", isAdmin.ToString(), ClaimValueTypes.Boolean),
                new Claim("avatarURL", user.AvatarURL ?? "null"),
                new Claim("isSocialAccount", user.IsSocialAccount.ToString(), ClaimValueTypes.Boolean)
            };
            //add roles
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["CustomAuth:IssuerSigningKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: Configuration["CustomAuth:Issuer"],
                audience: Configuration["CustomAuth:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> UpdateProfile(EditProfileViewModel model)
        {
            User user = await _userManager.FindByIdAsync(model.Id);
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.Description = model.Description;
            user.FavoriteTeamId = model.FavoriteTeamId;

            await _userManager.UpdateAsync(user);
            if (model.CurrentPassword.Length > 0 && model.NewPassword.Length > 0)
            {
                var result = _userManager.CheckPasswordAsync(user, model.CurrentPassword);
                if (!result.Result)
                    return false;

                await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            }
            return true;
        }

        public bool UploadAvatar(AvatarViewModel model)
        {
            string avatarDir = @"./ClientApp/build/content/images/avatars";
            if (!Directory.Exists(avatarDir))
                Directory.CreateDirectory(avatarDir);

            User user = _userManager.FindByIdAsync(model.Id).Result;
            string oldFilePath = avatarDir + "/" + user.AvatarURL + ".png";

            if (user.AvatarURL != null && File.Exists(oldFilePath))
                File.Delete(oldFilePath);

            string avatarId = Guid.NewGuid().ToString();
            string newFilePath = avatarDir + "/" + avatarId + ".png";
            user.AvatarURL = avatarId;
            _userManager.UpdateAsync(user).Wait();

            model.Avatar = model.Avatar.Substring(22);
            try
            {
                File.WriteAllBytes(newFilePath, Convert.FromBase64String(model.Avatar));
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool ClearAvatar(AvatarViewModel model)
        {
            string avatarDir = @"./ClientApp/build/content/images/avatars";
            User user = _userManager.FindByIdAsync(model.Id).Result;
            string avatarId = user.AvatarURL;

            if (avatarId == null)
                return true;

            if (Directory.Exists(avatarDir))
            {
                var filePath = avatarDir + "/" + avatarId + ".png";
                if (File.Exists(filePath))
                {
                    try
                    {
                        File.Delete(filePath);
                        File.Copy(@"./ClientApp/build/content/images/avatars/default.png", @"./ClientApp/build/content/images/avatars/" + model.Id + ".png");
                        user.AvatarURL = null;
                        _userManager.UpdateAsync(user).Wait();
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public async Task<bool> GoogleRegister(ClaimsPrincipal user)
        {
            string email = GetEmail(user);
            string username = GetUsernameFromEmail(email);
            Claim imageURL = user.Claims.Where(c => c.Type == "picture").FirstOrDefault();
            var newUser = new User
            {
                UserName = username,
                Email = email,
                IsSocialAccount = true
            };

            var result = await _userManager.CreateAsync(newUser);

            if (result.Succeeded)
            {
                if(imageURL != null)
                {
                    UploadAvatar(new AvatarViewModel()
                    {
                        Id = newUser.Id,
                        Avatar = await CommonFunctions.GetImageAsBase64Url(imageURL.Value)
                    });
                }
                await SendRegisterNotification(username);
            }

            return result.Succeeded;
        }

        public async Task<bool> GoogleLogin(ClaimsPrincipal user)
        {
            string email = GetEmail(user);
            User dbUser = await _userManager.FindByEmailAsync(email);
            if(!dbUser.IsSocialAccount)
            {
                dbUser.IsSocialAccount = true;
                IdentityResult result = await _userManager.UpdateAsync(dbUser);
                return result.Succeeded;
            }

            return true;
        }

        private string GetEmail(ClaimsPrincipal user)
        {
            List<Claim> claims = user.Claims.ToList();
            string email = claims[4].Value;
            return email;
        }

        private string GetUsernameFromEmail(string email)
        {
            int atIndex = email.IndexOf('@');
            string username = email.Substring(0, atIndex);
            return username;
        }

        private async Task SendRegisterNotification(string username)
        {
            PushNotificationViewModel notification =
                    new PushNotificationViewModel("Fantasy Hoops Admin Notification", string.Format("New user '{0}' just registerd in the system.", username))
                    {
                        Actions = new List<NotificationAction> { new NotificationAction("new_user", "👤 Profile") },
                        Data = new { userName = username }
                    };
            await _pushService.SendAdminNotification(notification);
        }

        public async Task<bool> DeleteProfile(ClaimsPrincipal user)
        {
            List<Claim> claims = user.Claims.ToList();
            string username = claims[1].Value;
            User userToDelete = await _userManager.FindByNameAsync(username);
            if(userToDelete != null)
            {

                ClearAvatar(new AvatarViewModel() { Id = user.Claims.ToList()[0].Value });
            }
            _userRepository.DeleteUserResources(userToDelete);
            IdentityResult result = await _userManager.DeleteAsync(userToDelete);
            return result.Succeeded;
        }
    }
}

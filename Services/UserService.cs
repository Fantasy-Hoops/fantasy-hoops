using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using fantasy_hoops.Database;
using fantasy_hoops.Models;
using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace fantasy_hoops.Services
{
    public class UserService : IUserService
    {

        private readonly GameContext _context;
        private readonly UserRepository _repository;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserService(GameContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _context = context;
            _repository = new UserRepository(_context);
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

            try
            {
                string avatarDir = @"./ClientApp/build/content/images/avatars";
                if (!Directory.Exists(avatarDir))
                    Directory.CreateDirectory(avatarDir);
                new WebClient().DownloadFile("https://fantasyhoops.org/content/images/avatars/default.png", @"./ClientApp/build/content/images/avatars/" + _repository.GetUserByName(model.UserName).Id + ".png");
            }
            catch { }

            PushNotificationViewModel notification =
                new PushNotificationViewModel("Fantasy Hoops Admin Notification", string.Format("New user '{0}' just registerd in the system.", model.UserName));
            notification.Actions = new List<NotificationAction> { new NotificationAction("new_user", "👤 Profile") };
            notification.Data = new { userName = model.UserName };
            await PushService.Instance.Value.SendAdminNotification(notification);

            return result.Succeeded;
        }

        public async void Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public string RequestToken(string username)
        {
            var user = _repository.GetUserByName(username);
            var roles = _repository.Roles(user.Id);
            bool isAdmin = _repository.IsAdmin(user.Id);

            var claims = new[]
            {
                new Claim("id", user.Id),
                new Claim("username", user.UserName),
                new Claim("email", user.Email),
                new Claim("description", user.Description ??""),
                new Claim("team", user.Team != null ? user.Team.Name : ""),
                new Claim("roles", string.Join(";", roles)),
                new Claim("isAdmin", isAdmin.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Tai yra raktas musu saugumo sistemai, kuo ilgesnis tuo geriau?"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "nekrosius.com",
                audience: "nekrosius.com",
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
            var filePath = avatarDir + "/" + model.Id + ".png";

            model.Avatar = model.Avatar.Substring(22);
            try
            {
                System.IO.File.WriteAllBytes(filePath, Convert.FromBase64String(model.Avatar));
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

            if (Directory.Exists(avatarDir))
            {
                var filePath = avatarDir + "/" + model.Id + ".png";
                if (System.IO.File.Exists(filePath))
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                        File.Copy(@"./ClientApp/build/content/images/avatars/default.png", @"./ClientApp/build/content/images/avatars/" + model.Id + ".png");
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            return true;
        }

    }
}

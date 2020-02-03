using System.Security.Claims;
using System.Threading.Tasks;
using fantasy_hoops.Models.ViewModels;

namespace fantasy_hoops.Services.Interfaces
{
    public interface IUserService
    {

        Task<bool> Login(LoginViewModel model);
        Task<bool> Register(RegisterViewModel user);
        Task<bool> GoogleLogin(ClaimsPrincipal user);
        Task<bool> GoogleRegister(ClaimsPrincipal model);
        void Logout();
        Task<string> RequestToken(string username);
        Task<string> RequestTokenById(string id);
        Task<string> RequestTokenByEmail(string id);
        Task<bool> UpdateProfile(EditProfileViewModel model);
        bool UploadAvatar(AvatarViewModel model);
        bool ClearAvatar(AvatarViewModel model);
        Task<bool> DeleteProfile(ClaimsPrincipal user);

    }
}

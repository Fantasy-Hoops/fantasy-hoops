using System.Threading.Tasks;
using fantasy_hoops.Models;
using fantasy_hoops.Models.ViewModels;

namespace fantasy_hoops.Services.Interfaces
{
    public interface IFriendService
    {

        void SendRequest(FriendRequestViewModel model);
        Task AcceptRequest(FriendRequestViewModel model, FriendRequest request);
        void CancelRequest(FriendRequestViewModel model, FriendRequest request);
        void RemoveRequest(FriendRequestViewModel model, FriendRequest request);

    }
}

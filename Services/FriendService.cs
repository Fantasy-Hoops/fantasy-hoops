using fantasy_hoops.Database;
using fantasy_hoops.Models;
using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Services.Interfaces;

namespace fantasy_hoops.Services
{
    public class FriendService : IFriendService
    {
        private readonly IFriendRepository _friendRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IPushService _pushService;
        private readonly UserManager<User> _userManager;

        public FriendService(IFriendRepository friendRepository, INotificationRepository notificationRepository, IPushService pushService, UserManager<User> userManager)
        {
            _friendRepository = friendRepository;
            _notificationRepository = notificationRepository;
            _pushService = pushService;
            _userManager = userManager;
        }

        public async Task AcceptRequest(FriendRequestViewModel model, FriendRequest request)
        {
            _friendRepository.UpdateRequest(request, model.SenderID, model.ReceiverID, RequestStatus.ACCEPTED);
            _notificationRepository.AddFriendRequestNotification(model.SenderID, model.ReceiverID, "Accepted your friend request.");
            User user = await _userManager.FindByIdAsync(model.ReceiverID);
            await _pushService.Send(model.SenderID, new PushNotificationViewModel("Fantasy Hoops Notification",
                string.Format("User '{0}' accepted your friend request.", user.UserName)));
        }

        public void CancelRequest(FriendRequestViewModel model, FriendRequest request)
        {
            _friendRepository.UpdateRequest(request, model.SenderID, model.ReceiverID, RequestStatus.CANCELED);

            _notificationRepository.RemoveFriendRequestNotification(model.ReceiverID, model.SenderID);
            _notificationRepository.RemoveFriendRequestNotification(model.SenderID, model.ReceiverID);
        }

        public void RemoveRequest(FriendRequestViewModel model, FriendRequest request)
        {
            _friendRepository.UpdateRequest(request, model.SenderID, model.ReceiverID, RequestStatus.CANCELED);

            _notificationRepository.RemoveFriendRequestNotification(model.SenderID, model.ReceiverID);
            _notificationRepository.RemoveFriendRequestNotification(model.ReceiverID, model.SenderID);
        }

        public async void SendRequest(FriendRequestViewModel model)
        {
            var request = _friendRepository.GetRequest(model.SenderID, model.ReceiverID);

            if (request == null)
                request = _friendRepository.GetRequest(model.ReceiverID, model.SenderID);

            if (request == null)
                _friendRepository.CreateRequest(model.SenderID, model.ReceiverID, RequestStatus.PENDING);
            else
                _friendRepository.UpdateRequest(request, model.SenderID, model.ReceiverID, RequestStatus.PENDING);

            _notificationRepository.AddFriendRequestNotification(model.ReceiverID, model.SenderID, "Sent you a friend request.");
        }

    }
}

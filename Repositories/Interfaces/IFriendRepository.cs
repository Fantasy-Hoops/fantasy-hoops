using System;
using System.Linq;
using fantasy_hoops.Models;

namespace fantasy_hoops.Repositories.Interfaces
{

    enum RequestType
    {
        Incoming,
        Outcoming
    }

    public interface IFriendRepository
    {
        RequestStatus GetStatus(String receiverID, String senderID);
        IQueryable<Object> GetRequests(String id);
        FriendRequest GetRequest(String senderID, String receiverID);
        void CreateRequest(String senderID, String receiverID, RequestStatus status);
        void UpdateRequest(FriendRequest request, String senderID, String receiverID, RequestStatus status);

    }
}

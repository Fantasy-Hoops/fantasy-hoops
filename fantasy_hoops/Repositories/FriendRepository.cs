using System;
using System.Linq;
using fantasy_hoops.Database;
using fantasy_hoops.Enums;
using fantasy_hoops.Models;
using fantasy_hoops.Repositories.Interfaces;

namespace fantasy_hoops.Repositories
{
    public class FriendRepository : IFriendRepository
    {

        private readonly GameContext _context;

        public FriendRepository()
        {
            _context = new GameContext();
        }

        public void CreateRequest(string senderID, string receiverID, RequestStatus status)
        {
            var request = new FriendRequest
            {
                SenderID = senderID,
                ReceiverID = receiverID,
                Date = DateTime.UtcNow,
                Status = status
            };
            _context.FriendRequests.Add(request);
            _context.SaveChanges();
        }

        public IQueryable<Object> GetRequests(string id)
        {
            return _context.FriendRequests
                .Where(x => x.SenderID.Equals(id) && x.Status.Equals(RequestStatus.PENDING))
                .Select(x => new
                {
                    x.Receiver.UserName,
                    x.Receiver.Id,
                    x.Receiver.AvatarURL,
                    Status = RequestType.Outcoming
                })
                .Union(_context.FriendRequests
                    .Where(x => x.ReceiverID.Equals(id) && x.Status.Equals(RequestStatus.PENDING))
                    .Select(x => new
                    {
                        x.Sender.UserName,
                        x.Sender.Id,
                        x.Sender.AvatarURL,
                        Status = RequestType.Incoming
                    })
                );
        }

        public IQueryable<Object> GetPendingRequests(string id)
        {            
            return _context.FriendRequests
                .Where(x => x.ReceiverID.Equals(id) && x.Status.Equals(RequestStatus.PENDING))
                .Select(x => new
                {
                    x.Sender.UserName,
                    x.Sender.Id,
                    x.Sender.AvatarURL
                });
        }

        public FriendRequest GetRequest(string senderID, string receiverID)
        {
            return _context.FriendRequests
                 .Where(x => x.SenderID.Equals(senderID) && x.ReceiverID.Equals(receiverID))
                 .FirstOrDefault();
        }

        public RequestStatus GetStatus(string receiverID, string senderID)
        {
            var request = GetRequest(senderID, receiverID);

            if (request == null)
            {
                request = GetRequest(receiverID, senderID);

                if (request != null && request.Status.Equals(RequestStatus.PENDING))
                    return RequestStatus.PENDING_INCOMING;
            }

            if (request == null)
                return RequestStatus.NO_REQUEST;
            return request.Status;
        }

        public void UpdateRequest(FriendRequest request, string senderID, string receiverID, RequestStatus status)
        {
            request.SenderID = senderID;
            request.ReceiverID = receiverID;
            request.Date = DateTime.UtcNow;
            request.Status = status;
            _context.SaveChanges();
        }

        public bool AreUsersFriends(string firstUserId, string secondUserId)
        {
            return _context.FriendRequests
                .Where(request => request.SenderID.Equals(firstUserId) && request.ReceiverID.Equals(secondUserId)
                                  || request.SenderID.Equals(secondUserId) && request.ReceiverID.Equals(firstUserId))
                .Any(request => request.Status == RequestStatus.ACCEPTED);
        }
    }
}

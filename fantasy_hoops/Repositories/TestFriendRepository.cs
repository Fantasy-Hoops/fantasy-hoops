﻿using System;
using System.Collections.Generic;
using System.Linq;
using fantasy_hoops.Models;
using fantasy_hoops.Models.Enums;
using fantasy_hoops.Repositories.Interfaces;

namespace fantasy_hoops.Repositories
{
    public class TestFriendRepository : IFriendRepository
    {
        readonly List<FriendRequest> requests;
        public TestFriendRepository()
        {
            requests = new List<FriendRequest>();
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
            requests.Add(request);
        }

        public IQueryable<Object> GetRequests(string id)
        {
            return requests
                .Where(x => x.SenderID.Equals(id) && x.Status.Equals(RequestStatus.PENDING))
                .Select(x => new
                {
                    x.Receiver.UserName,
                    x.Receiver.Id,
                    Status = RequestType.Outcoming
                })
                .Union(requests
                    .Where(x => x.ReceiverID.Equals(id) && x.Status.Equals(RequestStatus.PENDING))
                    .Select(x => new
                    {
                        x.Sender.UserName,
                        x.Sender.Id,
                        Status = RequestType.Incoming
                    })
                ).AsQueryable();
        }

        public FriendRequest GetRequest(string senderID, string receiverID)
        {
            return requests
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
        }

        public bool AreUsersFriends(string firstUserId, string secondUserId)
        {
            throw new NotImplementedException();
        }
    }
}

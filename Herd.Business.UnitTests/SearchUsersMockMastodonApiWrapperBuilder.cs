using Herd.Business.ApiWrappers;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.UnitTests
{
    public class SearchUsersMockMastodonApiWrapperBuilder
    {
        private HashSet<int> _userIDs = new HashSet<int>();
        private Dictionary<int, int> _followRelationships = new Dictionary<int, int>();

        public bool AllowAddContextToMastodonUserMethod { get; set; }
        public bool AllowAddContextToMastodonUsersMethod { get; set; }

        public void SetupUsers(params int[] userIDs)
        {
            foreach (var userID in userIDs)
            {
                _userIDs.Add(userID);
            }
        }

        public void SetupFollowRelationship(int followerUserID, int targetUserID)
        {
            _followRelationships[followerUserID] = targetUserID;
        }

        public Mock<IMastodonApiWrapper> BuildMockMastodonApiWrapper()
        {
            var mockMastodonApiWrapper = new Mock<IMastodonApiWrapper>();
            if (AllowAddContextToMastodonUserMethod)
            {
                // TODO: a million arguments is bad and not easily mockable. Wait until we take in one command arg
                // mockMastodonApiWrapper.Setup(a => a.AddContextToMastodonUser())
            }
            return mockMastodonApiWrapper;
        }
    }
}

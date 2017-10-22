using Herd.Business.ApiWrappers;
using Herd.Business.ApiWrappers.MastodonObjectContextOptions;
using Herd.Business.Models;
using Herd.Business.Models.Entities;
using Herd.Core;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Herd.Business.UnitTests
{
    public class SearchUsersMockMastodonApiWrapperBuilder
    {
        private Dictionary<string, HashSet<string>> _followRelationships = new Dictionary<string, HashSet<string>>();

        public string ActiveUserID { get; set; }

        public bool AllowAddContextToMastodonUserMethod { get; set; }
        public bool AllowAddContextToMastodonUsersMethod { get; set; }
        public bool AllowGetUsersByNameMethod { get; set; }

        public void SetupUsers(params int[] userIDs)
        {
            SetupUsers(userIDs.Select(id => id.ToString()).ToArray());
        }

        public void SetupUsers(params string[] userIDs)
        {
            foreach (var userID in userIDs)
            {
                if (!_followRelationships.ContainsKey(userID))
                {
                    _followRelationships[userID] = new HashSet<string>();
                }
            }
        }

        public void SetupFollowRelationship(int followerUserID, int targetUserID)
        {
            SetupFollowRelationship(followerUserID.ToString(), targetUserID.ToString());
        }

        public void SetupFollowRelationship(string followerUserID, string targetUserID)
        {
            _followRelationships[followerUserID].Add(targetUserID);
        }

        public Mock<IMastodonApiWrapper> BuildMockMastodonApiWrapper()
        {
            var mockMastodonApiWrapper = new Mock<IMastodonApiWrapper>();
            if (AllowAddContextToMastodonUserMethod)
            {
                mockMastodonApiWrapper
                    .Setup(a => a.AddContextToMastodonUser(It.IsAny<MastodonUser>(), It.IsAny<MastodonUserContextOptions>()))
                    .Returns<MastodonUser, MastodonUserContextOptions>(AddContextToMastodonUser);
            }
            if (AllowAddContextToMastodonUsersMethod)
            {
                mockMastodonApiWrapper
                    .Setup(a => a.AddContextToMastodonUsers(It.IsAny<IEnumerable<MastodonUser>>(), It.IsAny<MastodonUserContextOptions>()))
                    .Returns<IEnumerable<MastodonUser>, MastodonUserContextOptions>(AddContextToMastodonUsers);
            }
            if (AllowGetUsersByNameMethod)
            {
                mockMastodonApiWrapper
                    .Setup(a => a.GetUsersByName(It.IsAny<string>(), It.IsAny<MastodonUserContextOptions>(), It.IsAny<PagingOptions>()))
                    .Returns<string, MastodonUserContextOptions, PagingOptions>(GetUsersByName);
            }

            return mockMastodonApiWrapper;
        }

        #region Private Helpers

        private Task AddContextToMastodonUser(MastodonUser mastodonUser, MastodonUserContextOptions mastodonUserContextOptions)
        {
            var effectiveMastodonUserContextOptions = mastodonUserContextOptions ?? new MastodonUserContextOptions();
            if (effectiveMastodonUserContextOptions.IncludeFollowers)
            {
                mastodonUser.Followers = GetFollowers(mastodonUser.MastodonUserId).ToList();
            }
            if (effectiveMastodonUserContextOptions.IncludeFollowing)
            {
                mastodonUser.Following = GetFollowing(mastodonUser.MastodonUserId).ToList();
            }
            if (effectiveMastodonUserContextOptions.IncludeFollowsActiveUser)
            {
                mastodonUser.FollowsActiveUser = GetFollowingUserIDs(mastodonUser.MastodonUserId).Contains(ActiveUserID);
            }
            if (effectiveMastodonUserContextOptions.IncludeIsFollowedByActiveUser)
            {
                mastodonUser.IsFollowedByActiveUser = GetFollowingUserIDs(ActiveUserID).Contains(ActiveUserID);
            }
            return Task.CompletedTask;
        }

        private async Task AddContextToMastodonUsers(IEnumerable<MastodonUser> mastodonUsers, MastodonUserContextOptions mastodonUserContextOptions)
        {
            foreach (var mastodonUser in mastodonUsers)
            {
                await AddContextToMastodonUser(mastodonUser, mastodonUserContextOptions);
            }
        }

        private Task<IList<MastodonUser>> GetUsersByName(string name, MastodonUserContextOptions mastodonUserContextOptions, PagingOptions pagingOptions)
        {
            return Task.FromResult(GetUsers(mastodonUserContextOptions, null).Where(u => u.MastodonDisplayName.Contains(name, StringComparison.OrdinalIgnoreCase)).ToArray() as IList<MastodonUser>);
        }

        private IEnumerable<MastodonUser> GetUsers(MastodonUserContextOptions mastodonUserContextOptions, PagingOptions pagingOptions)
        {
            var mastodonUsers = _followRelationships.Keys.Select(BuildUser).ToArray();
            AddContextToMastodonUsers(mastodonUsers, mastodonUserContextOptions).Synchronously();
            return mastodonUsers;
        }

        private MastodonUser BuildUser(string id) => new MastodonUser
        {
            FollowersCount = GetFollowerUserIDs(id).Count(),
            FollowingCount = _followRelationships[id].Count,
            FollowsActiveUser = _followRelationships[id].Contains(ActiveUserID),
            IsFollowedByActiveUser = _followRelationships[ActiveUserID].Contains(id),
            MastodonDisplayName = $"User #{id}",
            MastodonUserId = id,
            MastodonUserName = $"user_{id}"
        };

        private IEnumerable<MastodonUser> GetFollowers(string targetUserID)
        {
            return GetFollowerUserIDs(targetUserID).Select(BuildUser);
        }

        private IEnumerable<string> GetFollowerUserIDs(string targetUserID)
        {
            return _followRelationships
                .Where(f => f.Value.Contains(targetUserID))
                .Select(f => f.Key);
        }

        private IEnumerable<MastodonUser> GetFollowing(string followerUserID)
        {
            return GetFollowingUserIDs(followerUserID).Select(BuildUser);
        }

        private IEnumerable<string> GetFollowingUserIDs(string followerUserID)
        {
            return _followRelationships[followerUserID];
        }

        #endregion
    }
}

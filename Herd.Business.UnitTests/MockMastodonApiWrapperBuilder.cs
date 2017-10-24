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
    public class MockMastodonApiWrapperBuilder
    {
        Dictionary<string, HashSet<string>> _followRelationships = new Dictionary<string, HashSet<string>>();
        Dictionary<string, string> _postsAndAuthors = new Dictionary<string, string>();
        Dictionary<string, string> _postParents = new Dictionary<string, string>();
        Dictionary<string, HashSet<string>> _postReplies = new Dictionary<string, HashSet<string>>();

        public string ActiveUserID { get; set; }

        // Users
        public bool AllowAddContextToMastodonUserMethod { get; set; }
        public bool AllowAddContextToMastodonUsersMethod { get; set; }
        public bool AllowGetUsersByNameMethod { get; set; }
        public bool AllowGetActiveUserMastodonAccountMethod { get; set; }
        public bool AllowGetMastodonAccountMethod { get; set; }
        public bool AllowGetFollowingMethod { get; set; }
        public bool AllowGetFollowersMethod { get; set; }

        // Posts
        public bool AllowAddContextToMastodonPostMethod { get; set; }
        public bool AllowAddContextToMastodonPostsMethod { get; set; }
        public bool AllowGetPostMethod { get; set; }
        public bool AllowGetPostsByAuthorUserIdMethod { get; set; }

        #region Users

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

            // Users
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
            if (AllowGetActiveUserMastodonAccountMethod)
            {
                mockMastodonApiWrapper
                    .Setup(a => a.GetActiveUserMastodonAccount(It.IsAny<MastodonUserContextOptions>()))
                    .Returns<MastodonUserContextOptions>(GetActiveUser);
            }
            if (AllowGetMastodonAccountMethod)
            {
                mockMastodonApiWrapper
                    .Setup(a => a.GetMastodonAccount(It.IsAny<string>(), It.IsAny<MastodonUserContextOptions>()))
                    .Returns<string, MastodonUserContextOptions>(GetUserByID);
            }
            if (AllowGetFollowingMethod)
            {
                mockMastodonApiWrapper
                    .Setup(a => a.GetFollowing(It.IsAny<string>(), It.IsAny<MastodonUserContextOptions>(), It.IsAny<PagingOptions>()))
                    .Returns<string, MastodonUserContextOptions, PagingOptions>(GetFollowing);
            }
            if (AllowGetFollowersMethod)
            {
                mockMastodonApiWrapper
                    .Setup(a => a.GetFollowers(It.IsAny<string>(), It.IsAny<MastodonUserContextOptions>(), It.IsAny<PagingOptions>()))
                    .Returns<string, MastodonUserContextOptions, PagingOptions>(GetFollowers);
            }

            // Posts
            if (AllowAddContextToMastodonPostMethod)
            {
                mockMastodonApiWrapper
                    .Setup(a => a.AddContextToMastodonPost(It.IsAny<MastodonPost>(), It.IsAny<MastodonPostContextOptions>()))
                    .Returns<MastodonPost, MastodonPostContextOptions>(AddContextToMastodonPost);
            }
            if (AllowAddContextToMastodonPostsMethod)
            {
                mockMastodonApiWrapper
                    .Setup(a => a.AddContextToMastodonPosts(It.IsAny<IEnumerable<MastodonPost>>(), It.IsAny<MastodonPostContextOptions>()))
                    .Returns<IEnumerable<MastodonPost>, MastodonPostContextOptions>(AddContextToMastodonPosts);
            }
            if (AllowGetPostMethod)
            {
                mockMastodonApiWrapper
                    .Setup(a => a.GetPost(It.IsAny<string>(), It.IsAny<MastodonPostContextOptions>()))
                    .Returns<string, MastodonPostContextOptions>(GetPost);
            }
            if (AllowGetPostsByAuthorUserIdMethod)
            {
                mockMastodonApiWrapper
                    .Setup(a => a.GetPostsByAuthorUserID(It.IsAny<string>(), It.IsAny<MastodonPostContextOptions>(), It.IsAny<PagingOptions>()))
                    .Returns<string, MastodonPostContextOptions, PagingOptions>(GetPostsByAuthorUserID);
            }

            return mockMastodonApiWrapper;
        }

        #endregion

        #region Posts

        public void CreatePost(int authorUserID, int postID, int? inReplyToPostID = null)
        {
            CreatePost(authorUserID.ToString(), postID.ToString(), inReplyToPostID?.ToString());
        }

        public void CreatePost(string authorUserID, string postID, string inReplyToPostID = null)
        {
            _postsAndAuthors[postID] = authorUserID;
            _postParents[postID] = inReplyToPostID;
            _postReplies[postID] = new HashSet<string>();
            if (!string.IsNullOrWhiteSpace(inReplyToPostID))
            {
                _postReplies[inReplyToPostID].Add(postID);
            }
        }

        #endregion

        #region Private Helpers

        #region Users

        private Task AddContextToMastodonUser(MastodonUser mastodonUser, MastodonUserContextOptions mastodonUserContextOptions)
        {
            var effectiveMastodonUserContextOptions = mastodonUserContextOptions ?? new MastodonUserContextOptions();
            if (effectiveMastodonUserContextOptions.IncludeFollowers)
            {
                mastodonUser.Followers = GetFollowerUsers(mastodonUser.MastodonUserId).ToList();
            }
            if (effectiveMastodonUserContextOptions.IncludeFollowing)
            {
                mastodonUser.Following = GetFollowingUsers(mastodonUser.MastodonUserId).ToList();
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

        private Task<MastodonUser> GetActiveUser(MastodonUserContextOptions mastodonUserContextOptions)
        {
            return GetUserByID(ActiveUserID, mastodonUserContextOptions);
        }

        private async Task<MastodonUser> GetUserByID(string id, MastodonUserContextOptions mastodonUserContextOptions)
        {
            var user = BuildUser(id);
            await AddContextToMastodonUser(user, mastodonUserContextOptions);
            return user;
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

        private async Task<IList<MastodonUser>> GetFollowing(string id, MastodonUserContextOptions mastodonUserContextOptions, PagingOptions pagingOptions)
        {
            var users = GetFollowingUsers(id).ToArray();
            await AddContextToMastodonUsers(users, mastodonUserContextOptions);
            return users;
        }

        private async Task<IList<MastodonUser>> GetFollowers(string id, MastodonUserContextOptions mastodonUserContextOptions, PagingOptions pagingOptions)
        {
            var users = GetFollowerUsers(id).ToArray();
            await AddContextToMastodonUsers(users, mastodonUserContextOptions);
            return users;
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

        private IEnumerable<MastodonUser> GetFollowerUsers(string targetUserID)
        {
            return GetFollowerUserIDs(targetUserID).Select(BuildUser);
        }

        private IEnumerable<string> GetFollowerUserIDs(string targetUserID)
        {
            return _followRelationships
                .Where(f => f.Value.Contains(targetUserID))
                .Select(f => f.Key);
        }

        private IEnumerable<MastodonUser> GetFollowingUsers(string followerUserID)
        {
            return GetFollowingUserIDs(followerUserID).Select(BuildUser);
        }

        private IEnumerable<string> GetFollowingUserIDs(string followerUserID)
        {
            return _followRelationships[followerUserID];
        }

        #endregion

        #region Posts

        async Task AddContextToMastodonPosts(IEnumerable<MastodonPost> posts, MastodonPostContextOptions mastodonPostContextOptions)
        {
            foreach (var post in posts)
            {
                await AddContextToMastodonPost(post, mastodonPostContextOptions);
            }
        }

        Task AddContextToMastodonPost(MastodonPost post, MastodonPostContextOptions mastodonPostContextOptions)
        {
            var effectiveMastodonPostContextOptions = mastodonPostContextOptions ?? new MastodonPostContextOptions();
            if (effectiveMastodonPostContextOptions.IncludeAncestors)
            {
                post.Ancestors = GetAncestorPostIDs(post.Id).Select(BuildPost).ToList();
            }
            if (effectiveMastodonPostContextOptions.IncludeDescendants)
            {
                post.Descendants = GetDescendantPostID(post.Id).Select(BuildPost).ToList();
            }
            return Task.CompletedTask;
        }

        async Task<MastodonPost> GetPost(string postID, MastodonPostContextOptions mastodonPostContextOptions)
        {
            var post = BuildPost(postID);
            await AddContextToMastodonPost(post, mastodonPostContextOptions);
            return post;
        }

        async Task<IList<MastodonPost>> GetPostsByAuthorUserID(string authorUserID, MastodonPostContextOptions mastodonPostContextOptions, PagingOptions pagingOptions)
        {
            var posts = _postsAndAuthors.Where(p => p.Value == authorUserID).Select(p => BuildPost(p.Key)).ToArray();
            foreach (var post in posts)
            {
                await AddContextToMastodonPost(post, mastodonPostContextOptions);
            }
            return posts;
        }

        IEnumerable<string> GetAncestorPostIDs(string targetPostID)
        {
            var parentPostID = _postParents[targetPostID];
            if (string.IsNullOrWhiteSpace(parentPostID))
            {
                yield return null;
            }

            yield return parentPostID;
            foreach (var ancestorPostID in GetAncestorPostIDs(parentPostID))
            {
                yield return ancestorPostID;
            }
        }

        IEnumerable<string> GetDescendantPostID(string targetPostID)
        {
            foreach (var replyPostID in _postReplies[targetPostID])
            {
                yield return replyPostID;
            }
            foreach (var replyPostID in _postReplies[targetPostID])
            {
                foreach (var descendantIdOfReplyPost in GetDescendantPostID(replyPostID))
                {
                    yield return descendantIdOfReplyPost;
                }
            }
        }

        MastodonPost BuildPost(string postID)
        {
            var postIdInt = int.Parse(postID);
            return new MastodonPost
            {
                Author = BuildUser(_postsAndAuthors[postID]),
                Content = $"Content for post {postID}",
                CreatedOnUTC = DateTime.UtcNow,
                Id = postID,
                InReplyToPostId = _postParents[postID],
                FavouritesCount = postIdInt,
                IsFavourited = postIdInt % 3 == 0,
                IsReblogged = postIdInt % 5 == 0,
                IsSensitive = postIdInt % 10 == 0,
                ReblogCount = postIdInt % 5 == 0 ? (postIdInt / 5) : 0,
                SpoilerText = postIdInt % 10 == 0 ? $"Spoiler text for post {postID}" : null,
                Visibility = (MastodonPostVisibility)(postIdInt % 4)
            };
        }

        #endregion

        #endregion
    }
}

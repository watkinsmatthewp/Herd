using Herd.Business.Models.Entities;
using Herd.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Herd.Business.ApiWrappers
{
    public interface IMastodonApiWrapper
    {
        #region Public properties

        string MastodonHostInstance { get; set; }
        Registration AppRegistration { get; set; }
        UserMastodonConnectionDetails UserMastodonConnectionDetails { get; set; }

        #endregion Public properties

        #region Auth

        Task<Registration> RegisterApp();
        string GetOAuthUrl(string redirectURL);
        Task<UserMastodonConnectionDetails> Connect(string token);

        #endregion Auth - Public methods

        #region User

        Task AddContextToMastodonUsers(IEnumerable<MastodonUser> mastodonUsers, bool includeFollowers = false, bool includeFollowing = false, bool includeIsFollowedByActiveUser = false, bool includeFollowsActiveUser = false);
        Task AddContextToMastodonUser(MastodonUser mastodonUser, bool includeFollowers = false, bool includeFollowing = false, bool includeIsFollowedByActiveUser = false, bool includeFollowsActiveUser = false);
        Task<IList<MastodonUser>> GetUsersByName(string name, bool includeFollowers = false, bool includeFollowing = false, bool includeIsFollowedByActiveUser = false, bool includeFollowsActiveUser = false, int? limit = 30);
        Task<MastodonUser> GetActiveUserMastodonAccount(bool includeFollowers = false, bool includeFollowing = false, bool includeIsFollowedByActiveUser = false, bool includeFollowsActiveUser = false);
        Task<MastodonUser> GetMastodonAccount(string id, bool includeFollowers = false, bool includeFollowing = false, bool includeIsFollowedByActiveUser = false, bool includeFollowsActiveUser = false);
        Task<IList<MastodonUser>> GetFollowing(string followerUserID, bool includeFollowers = false, bool includeFollowing = false, bool includeIsFollowedByActiveUser = false, bool includeFollowsActiveUser = false, int? limit = 30);
        Task<IList<MastodonUser>> GetFollowers(string followingUserID, bool includeFollowers = false, bool includeFollowing = false, bool includeIsFollowedByActiveUser = false, bool includeFollowsActiveUser = false, int? limit = 30);
        Task<MastodonRelationship> Follow(string userID, bool followUser);

        #endregion User

        #region Posts

        Task AddContextToMastodonPosts(IEnumerable<MastodonPost> mastodonPosts, bool includeAncestors = false, bool includeDescendants = false);
        Task AddContextToMastodonPost(MastodonPost mastodonPost, bool includeAncestors = false, bool includeDescendants = false);
        Task<MastodonPost> GetPost(string postID, bool includeAncestors = false, bool includeDescendants = false);
        Task<IList<MastodonPost>> GetPostsByAuthorUserID(string authorMastodonUserID, bool includeAncestors = false, bool includeDescendants = false, int? limit = 30);
        Task<IList<MastodonPost>> GetPostsByHashTag(string hashTag, bool includeAncestors = false, bool includeDescendants = false, int? limit = 30);
        Task<IList<MastodonPost>> GetPostsOnTimeline(bool includeAncestors = false, bool includeDescendants = false, int? limit = 30);
        Task<MastodonPost> CreateNewPost(string message, MastodonPostVisibility visibility, string replyStatusId = null, IEnumerable<string> mediaIds = null, bool sensitive = false, string spoilerText = null);

        #endregion Timeline Feeds
    }
}
using Herd.Business.Models.Entities;
using Herd.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Herd.Business.ApiWrappers
{
    public interface IMastodonApiWrapper
    {
        #region Public properties

        string MastodonHostInstance { get; }
        Registration AppRegistration { get; set; }
        UserMastodonConnectionDetails UserMastodonConnectionDetails { get; set; }

        #endregion Public properties

        #region Auth

        Task<Registration> RegisterApp();
        string GetOAuthUrl(string redirectURL);
        Task<UserMastodonConnectionDetails> Connect(string token);

        #endregion Auth - Public methods

        #region User

        Task<List<MastodonUser>> AddContextToMastodonUsers(List<MastodonUser> mastodonUsers, bool includeFollowers = false, bool includeFollowing = false, bool includeIsFollowedByActiveUser = false, bool includeFollowsActiveUser = false);
        Task<MastodonUser> AddContextToMastodonUser(MastodonUser mastodonUser, bool includeFollowers = false, bool includeFollowing = false, bool includeIsFollowedByActiveUser = false, bool includeFollowsActiveUser = false);
        Task<IList<MastodonUser>> GetUsersByName(string name, bool includeFollowers = false, bool includeFollowing = false, bool includeIsFollowedByActiveUser = false, bool includeFollowsActiveUser = false, int? limit = 30);
        Task<MastodonUser> GetActiveUserMastodonAccount(bool includeFollowers = false, bool includeFollowing = false, bool includeIsFollowedByActiveUser = false, bool includeFollowsActiveUser = false);
        Task<MastodonUser> GetMastodonAccount(long id, bool includeFollowers = false, bool includeFollowing = false, bool includeIsFollowedByActiveUser = false, bool includeFollowsActiveUser = false);
        Task<IList<MastodonUser>> GetFollowing(long followerUserID, bool includeFollowers = false, bool includeFollowing = false, bool includeIsFollowedByActiveUser = false, bool includeFollowsActiveUser = false, int? limit = 30);
        Task<IList<MastodonUser>> GetFollowers(long followingUserID, bool includeFollowers = false, bool includeFollowing = false, bool includeIsFollowedByActiveUser = false, bool includeFollowsActiveUser = false, int? limit = 30);
        Task<MastodonRelationship> Follow(long userID, bool followUser);

        #endregion User

        #region Timeline Feeds

        Task<List<MastodonPost>> GetRecentPosts(bool includeInReplyToPost = false, bool includeReplyPosts = false, long? maxID = null, long? sinceID = null, int? limit = 30);
        Task<MastodonPost> GetPost(long statusID, bool includeReplyPosts = false, bool includeReplyToPost = false);
        Task<MastodonPost> CreateNewPost(string message, MastodonPostVisibility visibility, long? replyStatusId = null, IEnumerable<long> mediaIds = null, bool sensitive = false, string spoilerText = null);
        
        #endregion Timeline Feeds
    }
}
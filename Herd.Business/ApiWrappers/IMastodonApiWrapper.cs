using Herd.Business.ApiWrappers.MastodonObjectContextOptions;
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

        Task AddContextToMastodonUsers(IEnumerable<MastodonUser> mastodonUsers, MastodonUserContextOptions mastodonUserContextOptions = null);
        Task AddContextToMastodonUser(MastodonUser mastodonUser, MastodonUserContextOptions mastodonUserContextOptions = null);
        Task<IList<MastodonUser>> GetUsersByName(string name, MastodonUserContextOptions mastodonUserContextOptions = null, int? limit = 30);
        Task<MastodonUser> GetActiveUserMastodonAccount(MastodonUserContextOptions mastodonUserContextOptions = null);
        Task<MastodonUser> GetMastodonAccount(string id, MastodonUserContextOptions mastodonUserContextOptions = null);
        Task<IList<MastodonUser>> GetFollowing(string followerUserID, MastodonUserContextOptions mastodonUserContextOptions = null, int? limit = 30);
        Task<IList<MastodonUser>> GetFollowers(string followingUserID, MastodonUserContextOptions mastodonUserContextOptions = null, int? limit = 30);
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
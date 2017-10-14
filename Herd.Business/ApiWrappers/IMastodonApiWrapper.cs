using Herd.Business.Models.Entities;
using Herd.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Herd.Business.ApiWrappers
{
    public interface IMastodonApiWrapper
    {
        string MastodonHostInstance { get; }
        Registration AppRegistration { get; set; }
        UserMastodonConnectionDetails UserMastodonConnectionDetails { get; set; }

        Task<Registration> RegisterApp();

        string GetOAuthUrl(string redirectURL);

        Task<UserMastodonConnectionDetails> Connect(string token);

        Task<MastodonUser> AddContextToMastodonUser(MastodonUser mastodonUser, bool includeFollowers = false, bool includeFollowing = false, bool includeIsFollowedByActiveUser = false, bool includeFollowsActiveUser = false);

        Task<IList<MastodonUser>> GetUsersByName(string name, bool includeFollowers = false, bool includeFollowing = false, bool includeIsFollowedByActiveUser = false, bool includeFollowsActiveUser = false, int? limit = 30);

        Task<MastodonUser> GetActiveUserMastodonAccount(bool includeFollowers = false, bool includeFollowing = false, bool includeIsFollowedByActiveUser = false, bool includeFollowsActiveUser = false);

        Task<MastodonUser> GetMastodonAccount(int id, bool includeFollowers = false, bool includeFollowing = false, bool includeIsFollowedByActiveUser = false, bool includeFollowsActiveUser = false);

        Task<IList<MastodonUser>> GetFollowing(int followerUserID, bool includeFollowers = false, bool includeFollowing = false, bool includeIsFollowedByActiveUser = false, bool includeFollowsActiveUser = false, int? limit = 30);

        Task<IList<MastodonUser>> GetFollowers(int followingUserID, bool includeFollowers = false, bool includeFollowing = false, bool includeIsFollowedByActiveUser = false, bool includeFollowsActiveUser = false, int? limit = 30);

        Task<List<MastodonPost>> GetRecentPosts(bool includeInReplyToPost = false, bool includeReplyPosts = false, int? maxID = null, int? sinceID = null, int? limit = 30);

        Task<MastodonPost> GetPost(int statusID, bool includeReplyPosts = false, bool includeReplyToPost = false);

        Task<MastodonPost> CreateNewPost(string message, MastodonPostVisibility visibility, int? replyStatusId = null, IEnumerable<int> mediaIds = null, bool sensitive = false, string spoilerText = null);
    }
}
using Herd.Business.Models.Entities;
using Herd.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Herd.Business
{
    public interface IMastodonApiWrapper
    {
        string MastodonHostInstance { get; }
        Registration AppRegistration { get; set; }
        UserMastodonConnectionDetails UserMastodonConnectionDetails { get; set; }

        Task<Registration> RegisterApp();

        string GetOAuthUrl(string redirectURL);

        Task<UserMastodonConnectionDetails> Connect(string token);

        Task<MastodonUser> GetActiveUserMastodonAccount(bool includeFollowers = false, bool includeFollowing = false);

        Task<List<MastodonPost>> GetRecentPosts(bool includeInReplyToPost = false, bool includeReplyPosts = false, int? maxID = null, int? sinceID = null, int? limit = 30);

        Task<MastodonPost> GetPost(int statusID, bool includeReplyPosts = false, bool includeReplyToPost = false);

        Task<MastodonPost> CreateNewPost(string message, MastodonPostVisibility visibility, int? replyStatusId = null, IEnumerable<int> mediaIds = null, bool sensitive = false, string spoilerText = null);
    }
}
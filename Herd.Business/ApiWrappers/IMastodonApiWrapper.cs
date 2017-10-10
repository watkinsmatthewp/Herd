using Herd.Data.Models;
using Mastonet.Entities;
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

        Task<Account> GetUserAccount();

        Task<IList<Status>> GetRecentStatuses(int limit = 30);

        Task<Status> GetStatus(int statusId);

        Task<Context> GetStatusContext(int statusId);

        Task<Status> CreateNewPost(string message, Mastonet.Visibility visibility, int? replyStatusId = null, IEnumerable<int> mediaIds = null, bool sensitive = false, string spoilerText = null);
    }
}
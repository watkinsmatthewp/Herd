using Herd.Data.Models;
using Mastonet.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Herd.Business
{
    public interface IMastodonApiWrapper
    {
        string MastodonHostInstance { get; }
        HerdAppRegistrationDataModel AppRegistration { get; set; }
        HerdUserMastodonConnectionDetails UserMastodonConnectionDetails { get; set; }

        Task<HerdAppRegistrationDataModel> RegisterApp();
        string GetOAuthUrl(string redirectURL);
        Task<HerdUserMastodonConnectionDetails> Connect(string token);

        Task<Account> GetUserAccount();
        Task<IList<Status>> GetRecentStatuses(int limit = 30);
        Task<Status> GetStatus(int statusId);
        Task<Context> GetStatusContext(int statusId);
        Task<Status> CreateNewPost(string message, Mastonet.Visibility visibility, int? replyStatusId = null, IEnumerable<int> mediaIds = null, bool sensitive = false, string spoilerText = null);
    }
}
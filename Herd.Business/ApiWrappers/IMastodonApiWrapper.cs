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
        Task<Status> CreateNewPost(string text);
    }
}
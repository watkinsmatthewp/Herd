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
        Task<Account> GetUserAccount();
        string GetOAuthUrl(string redirectURL);
        Task<IList<Status>> GetRecentStatuses(int limit = 30);
    }
}
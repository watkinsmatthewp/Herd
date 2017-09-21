using Herd.Data.Models;
using Mastonet.Entities;
using System.Threading.Tasks;

namespace Herd.Business
{
    public interface IMastodonApiWrapper
    {
        string MastodonHostInstance { get; }
        HerdAppRegistrationDataModel AppRegistration { get; set; }
        string UserApiToken { get; set; }

        Task<HerdAppRegistrationDataModel> RegisterApp();
        Task<Account> GetUserAccount();
        string GetOAuthUrl(string redirectURL);
    }
}
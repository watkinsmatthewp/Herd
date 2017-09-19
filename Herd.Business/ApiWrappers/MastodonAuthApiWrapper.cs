using System;
using System.Threading.Tasks;
using Herd.Data.Models;
using Mastonet;
using Mastonet.Entities;

namespace Herd.Business
{
    public partial class MastodonApiWrapper : IMastodonApiWrapper
    {
        private const Scope ALL_SCOPES = Scope.Read | Scope.Write | Scope.Follow;
        
        private Lazy<AuthenticationClient> _authClient = null;
        private AuthenticationClient AuthClient => _authClient.Value;

        #region Public methods

        public async Task<HerdAppRegistrationDataModel> RegisterApp()
        {
            var appRegistration = await AuthClient.CreateApp("Herd", ALL_SCOPES);
            return new HerdAppRegistrationDataModel
            {
                ClientId = appRegistration.ClientId,
                ClientSecret = appRegistration.ClientSecret,
                RegistrationID = appRegistration.Id,
                Instance = appRegistration.Instance
            };
        }

        public async Task<Account> GetUserAccount() => await ApiClient.GetCurrentUser();

        public string GetOAuthUrl(string redirectURL) => AuthClient.OAuthUrl(redirectURL);

        #endregion

        #region Private helper methods

        private AuthenticationClient LoadAuthenticationClient()
        {
            return AppRegistration == null ? new AuthenticationClient(MastodonHostInstance) : new AuthenticationClient(GetAppRegistration());
        }

        private MastodonClient LoadMastodonClient()
        {
            return new MastodonClient(GetAppRegistration(), new Auth
            {
                AccessToken = UserApiToken
            });
        }

        private AppRegistration GetAppRegistration()
        {
            return new AppRegistration
            {
                ClientId = AppRegistration.ClientId,
                ClientSecret = AppRegistration.ClientSecret,
                Id = AppRegistration.RegistrationID,
                Instance = AppRegistration.Instance,
                Scope = ALL_SCOPES
            };
        }

        #endregion
    }
}
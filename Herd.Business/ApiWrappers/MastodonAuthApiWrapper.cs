using Herd.Data.Models;
using Mastonet;
using Mastonet.Entities;
using System;
using System.Threading.Tasks;

namespace Herd.Business
{
    public partial class MastodonApiWrapper : IMastodonApiWrapper
    {
        private const Scope ALL_SCOPES = Scope.Read | Scope.Write | Scope.Follow;

        private Lazy<AuthenticationClient> _authClient;
        private AuthenticationClient AuthClient => _authClient.Value;

        #region Public methods

        public async Task<HerdAppRegistrationDataModel> RegisterApp()
        {
            var appRegistration = await AuthClient.CreateApp("Herd", ALL_SCOPES);
            return new HerdAppRegistrationDataModel
            {
                ClientId = appRegistration.ClientId,
                ClientSecret = appRegistration.ClientSecret,
                MastodonAppRegistrationID = appRegistration.Id,
                Instance = appRegistration.Instance
            };
        }

        public Task<Account> GetUserAccount() => ApiClient.GetCurrentUser();

        public string GetOAuthUrl(string redirectURL = null) => GetOAuthUrl(AuthClient, redirectURL);

        public static string GetOAuthUrl(string instance, string redirectURL = null) => GetOAuthUrl(new AuthenticationClient(instance), redirectURL);

        #endregion Public methods

        #region Private helper methods

        private static string GetOAuthUrl(AuthenticationClient authClient, string redirectURL)
        {
            return authClient.OAuthUrl(redirectURL);
        }

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
                Id = AppRegistration.MastodonAppRegistrationID,
                Instance = AppRegistration.Instance,
                Scope = ALL_SCOPES
            };
        }

        #endregion Private helper methods
    }
}
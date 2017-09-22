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

        private AuthenticationClient BuildMastodonAuthenticationClient()
        {
            if (string.IsNullOrWhiteSpace(MastodonHostInstance))
            {
                throw new ArgumentException($"{nameof(MastodonHostInstance)} cannot be null or empty");
            }
            return AppRegistration == null
                ? new AuthenticationClient(MastodonHostInstance)
                : new AuthenticationClient(AppRegistration.ToMastodonAppRegistration());
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
        private const Scope ALL_SCOPES = Scope.Read | Scope.Write | Scope.Follow;
        public async Task<HerdAppRegistrationDataModel> RegisterApp() => (await BuildMastodonAuthenticationClient().CreateApp("Herd", ALL_SCOPES)).ToHerdAppRegistration();
        public Task<Account> GetUserAccount() => BuildMastodonApiClient().GetCurrentUser();
        public string GetOAuthUrl(string redirectURL = null) => BuildMastodonAuthenticationClient().OAuthUrl(redirectURL);

        #endregion Public methods

        #endregion Private helper methods
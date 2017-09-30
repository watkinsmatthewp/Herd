using Herd.Data.Models;
using Mastonet;
using Mastonet.Entities;
using System;
using System.Threading.Tasks;

namespace Herd.Business
{
    public partial class MastodonApiWrapper : IMastodonApiWrapper
    {
        public const Scope ALL_SCOPES = Scope.Read | Scope.Write | Scope.Follow;

        #region Public methods

        public async Task<HerdAppRegistrationDataModel> RegisterApp() => (await BuildMastodonAuthenticationClient().CreateApp("Herd", ALL_SCOPES)).ToHerdAppRegistration();
        public Task<Account> GetUserAccount() => BuildMastodonApiClient().GetCurrentUser();
        public string GetOAuthUrl(string redirectURL = null) => BuildMastodonAuthenticationClient().OAuthUrl(redirectURL);
        public async Task<HerdUserMastodonConnectionDetails> Connect(string token) => (await BuildMastodonAuthenticationClient().ConnectWithCode(token)).ToHerdConnectionDetails(AppRegistration.ID);

        #endregion Public methods

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

        #endregion Private helper methods
    }
}
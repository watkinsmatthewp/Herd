using Mastonet;
using Mastonet.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Herd.Business
{
    public interface IMastodonApiWrapper
    {
        string HostInstance { get; }
        string UserApiToken { get; }

        Task<Account> GetUserAccount();
        Task<string> GetOAuthUrl(string redirectURL);
        Task<Boolean> LoginWithOAuthToken(string userApiToken);
    }

    public class MastodonApiWrapper : IMastodonApiWrapper
    {
        private AuthenticationClient _authClient = null;
        private MastodonClient _mastodonClient = null;

        public string HostInstance { get; private set; }
        public string UserApiToken { get; private set; }

        #region constructors
        public MastodonApiWrapper(string hostInstance)
            : this(hostInstance, null)
        {
        }

        public MastodonApiWrapper(string hostInstance, string userApiToken)
        {
            HostInstance = hostInstance;
            UserApiToken = userApiToken;

            _authClient = new AuthenticationClient(HostInstance);
        }
        #endregion 

        public async Task<Account> GetUserAccount()
        {
            var currentUser = await _mastodonClient.GetCurrentUser();
            return currentUser;
        }

        public async Task<string> GetOAuthUrl(string redirectURL)
        {
            // TODO: We should only have to register one time EVER for a user
            _authClient.AppRegistration = _authClient.AppRegistration ?? await CreateAppRegistration();
            return _authClient.OAuthUrl(redirectURL);
        }

        public async Task<bool> LoginWithOAuthToken(string userApiToken)
        {
            UserApiToken = userApiToken;
            Auth auth = await _authClient.ConnectWithCode(userApiToken);
            _mastodonClient = new MastodonClient(_authClient.AppRegistration, auth);
            return true;
        }

        #region Helper methods

        private async Task<AppRegistration> CreateAppRegistration() => await _authClient.CreateApp("Herd", Scope.Read | Scope.Write | Scope.Follow);

        #endregion
    }
}


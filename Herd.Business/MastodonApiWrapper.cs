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

        Task<string> GetOAuthUrl(string redirectURL);
        void SetUserApiToken(string userApiToken);
    }

    public class MastodonApiWrapper : IMastodonApiWrapper
    {
        private AuthenticationClient _authClient = null;

        public string HostInstance { get; private set; }
        public string UserApiToken { get; private set; }

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

        public async Task<string> GetOAuthUrl(string redirectURL)
        {
            _authClient.AppRegistration = _authClient.AppRegistration ?? await CreateAppRegistration();
            return _authClient.OAuthUrl(redirectURL);
        }

        public void SetUserApiToken(string userApiToken)
        {
            UserApiToken = userApiToken;
        }

        #region Helper methods

        private async Task<AppRegistration> CreateAppRegistration() => await _authClient.CreateApp("Herd", Scope.Read | Scope.Write | Scope.Follow);

        #endregion
    }
}


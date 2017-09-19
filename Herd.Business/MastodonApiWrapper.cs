using Mastonet;
using Mastonet.Entities;
using Herd.Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
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

    public class MastodonApiWrapper : IMastodonApiWrapper
    {
        private const Scope ALL_SCOPES = Scope.Read | Scope.Write | Scope.Follow;

        #region Private member variables

        private Lazy<AuthenticationClient> _authClient = null;
        private Lazy<MastodonClient> _mastodonClient = null;

        private AuthenticationClient AuthClient => _authClient.Value;
        private MastodonClient ApiClient => _mastodonClient.Value;

        #endregion

        #region Public properties

        public string MastodonHostInstance { get; private set; }
        public HerdAppRegistrationDataModel AppRegistration { get; set; }
        public string UserApiToken { get; set; }

        #endregion

        #region Constructors

        public MastodonApiWrapper(string mastodonHostInstance)
            : this(null as HerdAppRegistrationDataModel)
        {
            MastodonHostInstance = mastodonHostInstance;
        }

        public MastodonApiWrapper(HerdAppRegistrationDataModel registration)
            : this(registration, null)
        {

        }

        public MastodonApiWrapper(HerdAppRegistrationDataModel registration, string userApiToken)
        {
            AppRegistration = registration;
            MastodonHostInstance = AppRegistration?.Instance;
            UserApiToken = userApiToken;

            _authClient = new Lazy<AuthenticationClient>(LoadAuthenticationClient);
            _mastodonClient = new Lazy<MastodonClient>(LoadMastodonClient);
        }

        #endregion

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


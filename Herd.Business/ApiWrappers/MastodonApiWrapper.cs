using Herd.Data.Models;
using Mastonet;
using System;

namespace Herd.Business
{
    public partial class MastodonApiWrapper : IMastodonApiWrapper
    {
        #region Private member variables

        private Lazy<MastodonClient> _apiClient;
        private MastodonClient ApiClient => _apiClient.Value;

        #endregion Private member variables

        #region Public properties

        public string MastodonHostInstance { get; }
        public HerdAppRegistrationDataModel AppRegistration { get; set; }
        public string UserApiToken { get; set; }

        #endregion Public properties

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
            _apiClient = new Lazy<MastodonClient>(LoadMastodonClient);
        }

        #endregion Constructors
    }
}
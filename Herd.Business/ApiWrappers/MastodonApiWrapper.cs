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
    public partial class MastodonApiWrapper : IMastodonApiWrapper
    {
        #region Private member variables

        private Lazy<MastodonClient> _apiClient = null;
        private MastodonClient ApiClient => _apiClient.Value;

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
            _apiClient = new Lazy<MastodonClient>(LoadMastodonClient);
        }

        #endregion
    }
}


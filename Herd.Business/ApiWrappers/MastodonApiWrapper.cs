﻿using Herd.Data.Models;
using Mastonet;
using Mastonet.Entities;
using System;
using Herd.Core;

namespace Herd.Business
{
    public partial class MastodonApiWrapper : IMastodonApiWrapper
    {
        #region Public properties

        public string MastodonHostInstance { get; }
        public HerdAppRegistrationDataModel AppRegistration { get; set; }
        public string UserApiToken { get; set; }

        #endregion Public properties

        #region Constructors

        public MastodonApiWrapper()
            : this(null as string)
        {

        }

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
        }

        #endregion Constructors

        #region Private helper methods

        private MastodonClient BuildMastodonApiClient()
        {
            if (AppRegistration == null)
            {
                throw new ArgumentNullException(nameof(AppRegistration));
            }
            if (string.IsNullOrWhiteSpace(UserApiToken))
            {
                throw new ArgumentException($"{nameof(UserApiToken)} cannot be null or empty");
            }
            var authClient = BuildMastodonAuthenticationClient();
            return new MastodonClient(AppRegistration.ToMastodonAppRegistration(), authClient.ConnectWithCode(UserApiToken).Synchronously());
        }

        #endregion
    }
}
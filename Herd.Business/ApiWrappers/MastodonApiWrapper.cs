using Herd.Data.Models;
using Mastonet;
using Mastonet.Entities;
using System;
using System.Threading.Tasks;
using Herd.Core;
using System.Collections.Generic;
using System.Linq;

namespace Herd.Business
{
    public partial class MastodonApiWrapper : IMastodonApiWrapper
    {
        #region Public properties
        public string MastodonHostInstance { get; }
        public HerdAppRegistrationDataModel AppRegistration { get; set; }
        public HerdUserMastodonConnectionDetails UserMastodonConnectionDetails { get; set; }
        public const Scope ALL_SCOPES = Scope.Read | Scope.Write | Scope.Follow;
        #endregion Public properties

        #region Constructors
        public MastodonApiWrapper()
            : this(null as string){}

        public MastodonApiWrapper(string mastodonHostInstance)
            : this(null as HerdAppRegistrationDataModel)
        {
            MastodonHostInstance = mastodonHostInstance;
        }

        public MastodonApiWrapper(HerdAppRegistrationDataModel registration)
            : this(registration, null)
        {
        }

        public MastodonApiWrapper(HerdAppRegistrationDataModel registration, HerdUserMastodonConnectionDetails userMastodonConnectionDetails)
        {
            AppRegistration = registration;
            MastodonHostInstance = AppRegistration?.Instance;
            UserMastodonConnectionDetails = userMastodonConnectionDetails;
        }
        #endregion Constructors

        #region Private helper 
        private MastodonClient BuildMastodonApiClient()
        {
            if (AppRegistration == null)
            {
                throw new ArgumentNullException(nameof(AppRegistration));
            }
            if (UserMastodonConnectionDetails == null)
            {
                throw new ArgumentNullException(nameof(UserMastodonConnectionDetails));
            }
            return new MastodonClient(AppRegistration.ToMastodonAppRegistration(), UserMastodonConnectionDetails.ToMastodonAuth());
        }
        #endregion Private helper

        #region Auth Api 
        #region Auth - Public methods
        public async Task<HerdAppRegistrationDataModel> RegisterApp() => (await BuildMastodonAuthenticationClient().CreateApp("Herd", ALL_SCOPES)).ToHerdAppRegistration();
        public Task<Account> GetUserAccount() => BuildMastodonApiClient().GetCurrentUser();
        public string GetOAuthUrl(string redirectURL = null) => BuildMastodonAuthenticationClient().OAuthUrl(redirectURL);
        public async Task<HerdUserMastodonConnectionDetails> Connect(string token) => (await BuildMastodonAuthenticationClient().ConnectWithCode(token)).ToHerdConnectionDetails(AppRegistration.ID);
        #endregion Auth - Public methods

        #region Auth - Private mehtods
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
        #endregion Private methods
        #endregion Auth Api

        #region Timeline Feeds
        public async Task<System.Collections.Generic.IList<Status>> GetRecentStatuses(int limit = 30) => (await BuildMastodonApiClient().GetHomeTimeline(null, null, 30)).ToArray();
        public async Task<Status> GetStatus(int statusId) => (await BuildMastodonApiClient().GetStatus(statusId));
        public async Task<Context> GetStatusContext(int statusId) => (await BuildMastodonApiClient().GetStatusContext(statusId));
        public Task<Status> CreateNewPost(string message, Visibility visibility, int? replyStatusId = null, IEnumerable<int> mediaIds = null, bool sensitive = false, string spoilerText = null) => BuildMastodonApiClient().PostStatus(message, visibility, replyStatusId, mediaIds, sensitive, spoilerText);
        #endregion Timeline feeds
    }
}
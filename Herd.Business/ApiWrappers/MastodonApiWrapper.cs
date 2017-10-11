using Herd.Business.Models.Entities;
using Herd.Core;
using Herd.Data.Models;
using Mastonet;
using Mastonet.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Herd.Business
{
    public class MastodonApiWrapper : IMastodonApiWrapper
    {
        #region Public properties

        public string MastodonHostInstance { get; }
        public Registration AppRegistration { get; set; }
        public UserMastodonConnectionDetails UserMastodonConnectionDetails { get; set; }
        public const Scope ALL_SCOPES = Scope.Read | Scope.Write | Scope.Follow;

        #endregion Public properties

        #region Constructors

        public MastodonApiWrapper()
            : this(null as string) { }

        public MastodonApiWrapper(string mastodonHostInstance)
            : this(null as Registration)
        {
            MastodonHostInstance = mastodonHostInstance;
        }

        public MastodonApiWrapper(Registration registration)
            : this(registration, null)
        {
        }

        public MastodonApiWrapper(Registration registration, UserMastodonConnectionDetails userMastodonConnectionDetails)
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

        public async Task<Registration> RegisterApp() => (await BuildMastodonAuthenticationClient().CreateApp("Herd", ALL_SCOPES)).ToHerdAppRegistration();

        public string GetOAuthUrl(string redirectURL = null) => BuildMastodonAuthenticationClient().OAuthUrl(redirectURL);

        public async Task<UserMastodonConnectionDetails> Connect(string token)
        {
            UserMastodonConnectionDetails = (await BuildMastodonAuthenticationClient().ConnectWithCode(token)).ToHerdConnectionDetails(AppRegistration.ID, -1);
            UserMastodonConnectionDetails.MastodonUserID = (await BuildMastodonApiClient().GetCurrentUser()).Id;
            return UserMastodonConnectionDetails;
        }

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

        #endregion Auth - Private mehtods

        #endregion Auth Api

        #region User

        public async Task<MastodonUser> GetActiveUserMastodonAccount(bool includeFollowers = false, bool includeFollowing = false)
        {
            var mastodonClient = BuildMastodonApiClient();
            var mastodonUser = (await mastodonClient.GetCurrentUser()).ToMastodonUser();
            if (includeFollowers)
            {
                mastodonUser.Followers = (await mastodonClient.GetAccountFollowers(mastodonUser.MastodonUserId))
                    .Select(u => u.ToMastodonUser().And(mu => mu.FollowsRelevantUser = true)).ToList();
            }
            if (includeFollowing)
            {
                mastodonUser.Following = (await mastodonClient.GetAccountFollowing(mastodonUser.MastodonUserId))
                    .Select(u => u.ToMastodonUser().And(mu => mu.IsFollowedByRelevantUser = true)).ToList();
            }
            return mastodonUser;
        }

        #endregion User

        #region Timeline Feeds

        public async Task<List<MastodonPost>> GetRecentPosts(bool includeInReplyToPost = false, bool includeReplyPosts = false, int? maxID = null, int? sinceID = null, int? limit = 30)
        {
            var posts = new List<MastodonPost>();
            var mastodonClient = BuildMastodonApiClient();

            foreach (var mastodonStatus in await mastodonClient.GetHomeTimeline(maxID, sinceID, limit))
            {
                posts.Add(await GetContextualPost(mastodonClient, mastodonStatus, includeReplyPosts, includeInReplyToPost));
            }

            return posts;
        }

        public async Task<MastodonPost> GetPost(int statusID, bool includeAncestors = false, bool includeDescendants = false)
        {
            var mastodonClient = BuildMastodonApiClient();
            var status = await mastodonClient.GetStatus(statusID);
            return await GetContextualPost(mastodonClient, status, includeAncestors, includeDescendants);
        }

        public async Task<MastodonPost> CreateNewPost(string message, MastodonPostVisibility visibility, int? replyStatusId = null, IEnumerable<int> mediaIds = null, bool sensitive = false, string spoilerText = null)
        {
            return (await BuildMastodonApiClient().PostStatus(message, visibility.ToVisibility(), replyStatusId, mediaIds, sensitive, spoilerText)).ToPost();
        }

        #endregion Timeline Feeds

        #region Private helpers

        private async Task<MastodonPost> GetContextualPost(MastodonClient mastodonClient, Status mastodonStatus, bool includeAncestors, bool includeDescendants)
        {
            var post = mastodonStatus.ToPost();
            var setAncestors = includeAncestors && mastodonStatus.InReplyToId.HasValue;
            var setDescendants = includeDescendants;

            if (setAncestors || setDescendants)
            {
                var statusContext = await mastodonClient.GetStatusContext(mastodonStatus.Id);
                if (setAncestors)
                {
                    post.Ancestors = statusContext.Ancestors.Select(s => s.ToPost()).ToList();
                }
                if (setDescendants)
                {
                    post.Descendants = statusContext.Descendants.Select(s => s.ToPost()).ToList();
                }
            }

            return post;
        }

        #endregion Private helpers
    }
}
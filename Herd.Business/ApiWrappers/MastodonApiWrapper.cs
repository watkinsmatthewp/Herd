using Herd.Business.Models.Entities;
using Herd.Core;
using Herd.Data.Models;
using Mastonet;
using Mastonet.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Herd.Business.ApiWrappers
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

        public async Task<MastodonUser> GetActiveUserMastodonAccount(bool includeFollowers = false, bool includeFollowing = false, bool includeIsFollowedByActiveUser = false, bool includeFollowsActiveUser = false)
        {
            var mastodonClient = BuildMastodonApiClient();
            var mastodonUser = (await mastodonClient.GetCurrentUser()).ToMastodonUser();
            mastodonUser = await AddContextToMastodonUser(mastodonUser, includeFollowers, includeFollowing, includeIsFollowedByActiveUser, includeFollowsActiveUser);
            return mastodonUser;
        }

        public async Task<MastodonUser> GetMastodonAccount(int id, bool includeFollowers = false, bool includeFollowing = false, bool includeIsFollowedByActiveUser = false, bool includeFollowsActiveUser = false)
        {
            var mastodonClient = BuildMastodonApiClient();
            var mastodonUser = (await mastodonClient.GetAccount(id)).ToMastodonUser();
            mastodonUser = await AddContextToMastodonUser(mastodonUser, includeFollowers, includeFollowing, includeIsFollowedByActiveUser, includeFollowsActiveUser);
            return mastodonUser;
        }

        public async Task<IList<MastodonUser>> GetUsersByName(string name, bool includeFollowers = false, bool includeFollowing = false, bool includeIsFollowedByActiveUser = false, bool includeFollowsActiveUser = false, int? limit = 30)
        {
            var mastodonClient = BuildMastodonApiClient();
            var mastodonUsers = (await mastodonClient.SearchAccounts(name, null, null, limit)).Select(u => u.ToMastodonUser()).ToList();
            await AddContextToMastodonUsers(mastodonUsers, includeFollowers, includeFollowing, includeIsFollowedByActiveUser, includeFollowsActiveUser);
            return mastodonUsers;
        }

        public async Task<IList<MastodonUser>> GetFollowing(int followerUserID, bool includeFollowers = false, bool includeFollowing = false, bool includeIsFollowedByActiveUser = false, bool includeFollowsActiveUser = false, int? limit = 30)
        {
            var mastodonClient = BuildMastodonApiClient();
            var mastodonUsers = (await mastodonClient.GetAccountFollowing(followerUserID, null, null, limit)).Select(u => u.ToMastodonUser()).ToList();
            await AddContextToMastodonUsers(mastodonUsers, includeFollowers, includeFollowing, includeIsFollowedByActiveUser, includeFollowsActiveUser);
            return mastodonUsers;
        }

        public async Task<IList<MastodonUser>> GetFollowers(int followingUserID, bool includeFollowers = false, bool includeFollowing = false, bool includeIsFollowedByActiveUser = false, bool includeFollowsActiveUser = false, int? limit = 30)
        {
            var mastodonClient = BuildMastodonApiClient();
            var mastodonUsers = (await mastodonClient.GetAccountFollowers(followingUserID, null, null, limit)).Select(u => u.ToMastodonUser()).ToList();
            await AddContextToMastodonUsers(mastodonUsers, includeFollowers, includeFollowing, includeIsFollowedByActiveUser, includeFollowsActiveUser);
            return mastodonUsers;
        }

        public async Task<List<MastodonUser>> AddContextToMastodonUsers(List<MastodonUser> mastodonUsers, bool includeFollowers = false, bool includeFollowing = false, bool includeIsFollowedByActiveUser = false, bool includeFollowsActiveUser = false)
        {
            var mastodonClient = BuildMastodonApiClient();

            foreach (var mastodonUser in mastodonUsers)
            {
                if (includeFollowers)
                {
                    // Get the follower of this user
                    mastodonUser.Followers = (await mastodonClient.GetAccountFollowers(mastodonUser.MastodonUserId))
                        .Select(u => u.ToMastodonUser().Then(mu => mu.FollowsRelevantUser = true)).ToList();
                    mastodonUser.IsFollowedByActiveUser = mastodonUser.Followers.Any(f => f.MastodonUserId == UserMastodonConnectionDetails.MastodonUserID);
                }
                if (includeFollowing)
                {
                    // Get the users this user us following
                    mastodonUser.Following = (await mastodonClient.GetAccountFollowing(mastodonUser.MastodonUserId))
                        .Select(u => u.ToMastodonUser().Then(mu => mu.IsFollowedByRelevantUser = true)).ToList();
                    mastodonUser.FollowsActiveUser = mastodonUser.Following.Any(f => f.MastodonUserId == UserMastodonConnectionDetails.MastodonUserID);
                }
            }

            if ((includeIsFollowedByActiveUser || includeFollowsActiveUser) && (!includeFollowers || !includeFollowing))
            {
                // We didn't fetch both the full followers and full following lists for the users. But we still need to know
                // whether these users follow or are followed by the active user. Fortunately we can make a single API call for this.

                // TODO: currently we assume that the relationships are returned 1 for each user queried in the order that they were queried.
                // But this is extremely dangerous and likely to be wrong. Match on ID when https://github.com/glacasa/Mastonet/issues/23
                // gets merged into MastoNet and the NuGet package gets updated.
                mastodonUsers = mastodonUsers.OrderBy(u => u.MastodonUserId).ToList();
                var relationships = (await mastodonClient.GetAccountRelationships(mastodonUsers.Select(u => u.MastodonUserId))).ToList();
                if (relationships.Count != mastodonUsers.Count)
                {
                    throw new Exception("Unable to guess relationships. Waiting on @glacasa to update the NuGet package from https://github.com/glacasa/Mastonet/issues/23");
                }
                for (var i = 0; i < mastodonUsers.Count; i++)
                {
                    mastodonUsers[i].FollowsActiveUser = relationships[i].FollowedBy;
                    mastodonUsers[i].IsFollowedByActiveUser = relationships[i].Following;
                }
            }

            return mastodonUsers;
        }

        public async Task<MastodonUser> AddContextToMastodonUser(MastodonUser mastodonUser, bool includeFollowers = false, bool includeFollowing = false, bool includeIsFollowedByActiveUser = false, bool includeFollowsActiveUser = false)
        {
            var mastodonClient = BuildMastodonApiClient();

            if (includeFollowers)
            {
                // Get the follower of this user
                mastodonUser.Followers = (await mastodonClient.GetAccountFollowers(mastodonUser.MastodonUserId))
                    .Select(u => u.ToMastodonUser().Then(mu => mu.FollowsRelevantUser = true)).ToList();
                mastodonUser.IsFollowedByActiveUser = mastodonUser.Followers.Any(f => f.MastodonUserId == UserMastodonConnectionDetails.MastodonUserID);
            }
            if (includeFollowing)
            {
                // Get the users this user us following
                mastodonUser.Following = (await mastodonClient.GetAccountFollowing(mastodonUser.MastodonUserId))
                    .Select(u => u.ToMastodonUser().Then(mu => mu.IsFollowedByRelevantUser = true)).ToList();
                mastodonUser.FollowsActiveUser = mastodonUser.Following.Any(f => f.MastodonUserId == UserMastodonConnectionDetails.MastodonUserID);
            }

            if ((includeIsFollowedByActiveUser || includeFollowsActiveUser) && (!mastodonUser.IsFollowedByActiveUser.HasValue || !mastodonUser.FollowsActiveUser.HasValue))
            {
                // We haven't gotten any follow data, but we still need to know if this user has a relationship with the active user
                var userRelationships = (await mastodonClient.GetAccountRelationships(mastodonUser.MastodonUserId)).ToArray();
                mastodonUser.FollowsActiveUser = userRelationships.Length == 1 ? userRelationships[0].FollowedBy : false;
                mastodonUser.IsFollowedByActiveUser = userRelationships.Length == 1 ? userRelationships[0].Following : false;
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
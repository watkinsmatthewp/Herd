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

        #region Auth

        #region Auth - Public methods

        public async Task<Registration> RegisterApp() => (await BuildMastodonAuthenticationClient().CreateApp("Herd", ALL_SCOPES)).ToHerdAppRegistration();

        public string GetOAuthUrl(string redirectURL = null) => BuildMastodonAuthenticationClient().OAuthUrl(redirectURL);

        public async Task<UserMastodonConnectionDetails> Connect(string token)
        {
            UserMastodonConnectionDetails = (await BuildMastodonAuthenticationClient().ConnectWithCode(token)).ToHerdConnectionDetails(AppRegistration.ID, "-1");
            UserMastodonConnectionDetails.MastodonUserID = (await BuildMastodonApiClient().GetCurrentUser()).Id.ToString();
            return UserMastodonConnectionDetails;
        }

        #endregion Auth - Public methods

        #region Auth - Private methods

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

        public async Task<MastodonUser> GetMastodonAccount(string id, bool includeFollowers = false, bool includeFollowing = false, bool includeIsFollowedByActiveUser = false, bool includeFollowsActiveUser = false)
        {
            var mastodonClient = BuildMastodonApiClient();
            var mastodonUser = (await mastodonClient.GetAccount(id.ToLong())).ToMastodonUser();
            mastodonUser = await AddContextToMastodonUser(mastodonUser, includeFollowers, includeFollowing, includeIsFollowedByActiveUser, includeFollowsActiveUser);
            return mastodonUser;
        }

        public async Task<IList<MastodonUser>> GetUsersByName(string name, bool includeFollowers = false, bool includeFollowing = false, bool includeIsFollowedByActiveUser = false, bool includeFollowsActiveUser = false, int? limit = 30)
        {
            var mastodonClient = BuildMastodonApiClient();
            var mastodonUsers = (await mastodonClient.SearchAccounts(name, limit)).Select(u => u.ToMastodonUser()).ToList();
            await AddContextToMastodonUsers(mastodonUsers, includeFollowers, includeFollowing, includeIsFollowedByActiveUser, includeFollowsActiveUser);
            return mastodonUsers;
        }

        public async Task<IList<MastodonUser>> GetFollowing(string followerUserID, bool includeFollowers = false, bool includeFollowing = false, bool includeIsFollowedByActiveUser = false, bool includeFollowsActiveUser = false, int? limit = 30)
        {
            var mastodonClient = BuildMastodonApiClient();
            var mastodonUsers = (await mastodonClient.GetAccountFollowing(followerUserID.ToLong(), null, null, limit)).Select(u => u.ToMastodonUser()).ToList();
            await AddContextToMastodonUsers(mastodonUsers, includeFollowers, includeFollowing, includeIsFollowedByActiveUser, includeFollowsActiveUser);
            return mastodonUsers;
        }

        public async Task<IList<MastodonUser>> GetFollowers(string followingUserID, bool includeFollowers = false, bool includeFollowing = false, bool includeIsFollowedByActiveUser = false, bool includeFollowsActiveUser = false, int? limit = 30)
        {
            var mastodonClient = BuildMastodonApiClient();
            var mastodonUsers = (await mastodonClient.GetAccountFollowers(followingUserID.ToLong(), null, null, limit)).Select(u => u.ToMastodonUser()).ToList();
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
                    mastodonUser.Followers = (await mastodonClient.GetAccountFollowers(mastodonUser.MastodonUserId.ToLong()))
                        .Select(u => u.ToMastodonUser().Then(mu => mu.FollowsRelevantUser = true)).ToList();
                    mastodonUser.IsFollowedByActiveUser = mastodonUser.Followers.Any(f => f.MastodonUserId == UserMastodonConnectionDetails.MastodonUserID);
                }
                if (includeFollowing)
                {
                    // Get the users this user us following
                    mastodonUser.Following = (await mastodonClient.GetAccountFollowing(mastodonUser.MastodonUserId.ToLong()))
                        .Select(u => u.ToMastodonUser().Then(mu => mu.IsFollowedByRelevantUser = true)).ToList();
                    mastodonUser.FollowsActiveUser = mastodonUser.Following.Any(f => f.MastodonUserId == UserMastodonConnectionDetails.MastodonUserID);
                }
            }

            if ((includeIsFollowedByActiveUser || includeFollowsActiveUser) && (!includeFollowers || !includeFollowing))
            {
                // We didn't fetch both the full followers and full following lists for the users. But we still need to know
                // whether these users follow or are followed by the active user. Fortunately we can make a single API call for this.
                var relationships = (await mastodonClient.GetAccountRelationships(mastodonUsers.Select(u => u.MastodonUserId.ToLong()))).Select(r => r.ToMastodonRelationship()).ToArray();
                foreach (var mastodonUser in mastodonUsers)
                {
                    var relevantRelationships = relationships.Where(r => r.ID == mastodonUser.MastodonUserId).ToArray();
                    mastodonUser.FollowsActiveUser = relevantRelationships.Any(r => r.FollowedBy);
                    mastodonUser.IsFollowedByActiveUser = relevantRelationships.Any(r => r.Following);
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
                mastodonUser.Followers = (await mastodonClient.GetAccountFollowers(mastodonUser.MastodonUserId.ToLong()))
                    .Select(u => u.ToMastodonUser().Then(mu => mu.FollowsRelevantUser = true)).ToList();
                mastodonUser.IsFollowedByActiveUser = mastodonUser.Followers.Any(f => f.MastodonUserId == UserMastodonConnectionDetails.MastodonUserID);
            }
            if (includeFollowing)
            {
                // Get the users this user us following
                mastodonUser.Following = (await mastodonClient.GetAccountFollowing(mastodonUser.MastodonUserId.ToLong()))
                    .Select(u => u.ToMastodonUser().Then(mu => mu.IsFollowedByRelevantUser = true)).ToList();
                mastodonUser.FollowsActiveUser = mastodonUser.Following.Any(f => f.MastodonUserId == UserMastodonConnectionDetails.MastodonUserID);
            }

            if ((includeIsFollowedByActiveUser || includeFollowsActiveUser) && (!mastodonUser.IsFollowedByActiveUser.HasValue || !mastodonUser.FollowsActiveUser.HasValue))
            {
                // We haven't gotten any follow data, but we still need to know if this user has a relationship with the active user
                var userRelationships = (await mastodonClient.GetAccountRelationships(mastodonUser.MastodonUserId.ToLong())).ToArray();
                mastodonUser.FollowsActiveUser = userRelationships.Length == 1 ? userRelationships[0].FollowedBy : false;
                mastodonUser.IsFollowedByActiveUser = userRelationships.Length == 1 ? userRelationships[0].Following : false;
            }

            return mastodonUser;
        }

        public async Task<MastodonRelationship> Follow(string userID, bool followUser)
        {
            if(followUser)
            {
                return (await BuildMastodonApiClient().Follow(userID.ToLong())).ToMastodonRelationship();
            } else
            {
                return (await BuildMastodonApiClient().Unfollow(userID.ToLong())).ToMastodonRelationship();
            }
            
        }

        #endregion User

        #region Posts

        public async Task<List<MastodonPost>> AddContextToMastodonPosts(List<MastodonPost> mastodonPosts, bool includeAncestors = false, bool includeDescendants = false)
        {
            for (var i = 0; i < mastodonPosts.Count; i++)
            {
                mastodonPosts[i] = await AddContextToMastodonPost(mastodonPosts[i], includeAncestors, includeDescendants);
            }
            return mastodonPosts;
        }

        public async Task<MastodonPost> AddContextToMastodonPost(MastodonPost mastodonPost, bool includeAncestors = false, bool includeDescendants = false)
        {
            var mastodonClient = BuildMastodonApiClient();
            if (includeAncestors || includeDescendants)
            {
                var statusContext = await mastodonClient.GetStatusContext(mastodonPost.Id.ToLong());
                if (includeAncestors)
                {
                    mastodonPost.Ancestors = statusContext.Ancestors.Select(s => s.ToPost()).ToList();
                }
                if (includeDescendants)
                {
                    mastodonPost.Descendants = statusContext.Descendants.Select(s => s.ToPost()).ToList();
                }
            }
            return mastodonPost;
        }

        public async Task<MastodonPost> GetPost(string postID, bool includeAncestors = false, bool includeDescendants = false)
        {
            var mastodonClient = BuildMastodonApiClient();
            var post = (await mastodonClient.GetStatus(postID.ToLong())).ToPost();
            return await AddContextToMastodonPost(post, includeAncestors, includeDescendants);
        }

        public async Task<List<MastodonPost>> GetPostsByAuthorUserID(string authorMastodonUserID, bool includeAncestors = false, bool includeDescendants = false, int? limit = 30)
        {
            var mastodonClient = BuildMastodonApiClient();
            var posts = (await mastodonClient.GetAccountStatuses(authorMastodonUserID.ToLong(), null, null, limit, false, false)).Select(s => s.ToPost()).ToList();
            return await AddContextToMastodonPosts(posts, includeAncestors, includeDescendants);
        }

        public async Task<List<MastodonPost>> GetPostsByHashTag(string hashTag, bool includeAncestors = false, bool includeDescendants = false, int? limit = 30)
        {
            var mastodonClient = BuildMastodonApiClient();
            var posts = (await mastodonClient.GetTagTimeline(hashTag, null, null, limit)).Select(s => s.ToPost()).ToList();
            return await AddContextToMastodonPosts(posts, includeAncestors, includeDescendants);
        }

        public async Task<List<MastodonPost>> GetPostsOnTimeline(bool includeAncestors = false, bool includeDescendants = false, int? limit = 30)
        {
            var mastodonClient = BuildMastodonApiClient();
            var posts = (await mastodonClient.GetHomeTimeline(null, null, limit)).Select(s => s.ToPost()).ToList();
            return await AddContextToMastodonPosts(posts, includeAncestors, includeDescendants);
        }

        public async Task<MastodonPost> CreateNewPost(string message, MastodonPostVisibility visibility, string replyStatusId = null, IEnumerable<string> mediaIds = null, bool sensitive = false, string spoilerText = null)
        {
            var mastodonClient = BuildMastodonApiClient();
            return (await mastodonClient.PostStatus(message, visibility.ToVisibility(), replyStatusId.ToNullableLong(), mediaIds.ToLongs(), sensitive, spoilerText)).ToPost();
        }

        #endregion Posts
    }
}
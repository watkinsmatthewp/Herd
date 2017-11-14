using Herd.Business.ApiWrappers.MastodonObjectContextOptions;
using Herd.Business.Extensions;
using Herd.Business.Models;
using Herd.Business.Models.Entities;
using Herd.Core;
using Herd.Data.Models;
using Mastonet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Herd.Business.ApiWrappers
{
    public class MastodonApiWrapper : IMastodonApiWrapper
    {
        #region Public properties

        public string MastodonHostInstance { get; set; }
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

        #endregion Auth - Private methods

        #endregion Auth

        #region User

        public async Task<MastodonUser> GetActiveUserMastodonAccount(MastodonUserContextOptions mastodonUserContextOptions = null)
        {
            var mastodonClient = BuildMastodonApiClient();
            var mastodonUser = (await mastodonClient.GetCurrentUser()).ToMastodonUser();
            await AddContextToMastodonUser(mastodonUser, mastodonUserContextOptions);
            return mastodonUser;
        }

        public async Task<MastodonUser> GetMastodonAccount(string id, MastodonUserContextOptions mastodonUserContextOptions = null)
        {
            var mastodonClient = BuildMastodonApiClient();
            var mastodonUser = (await mastodonClient.GetAccount(id.ToLong())).ToMastodonUser();
            await AddContextToMastodonUser(mastodonUser, mastodonUserContextOptions);
            return mastodonUser;
        }

        public async Task<IList<MastodonUser>> GetUsersByName(string name, MastodonUserContextOptions mastodonUserContextOptions = null, PagingOptions pagingOptions = null)
        {
            var effectivePagingOptions = pagingOptions ?? new PagingOptions();
            var mastodonClient = BuildMastodonApiClient();
            var mastodonUsersApiTask = mastodonClient.SearchAccounts(name, effectivePagingOptions.Limit);
            var mastodonUsers = (await mastodonUsersApiTask).Select(u => u.ToMastodonUser()).ToList();
            await AddContextToMastodonUsers(mastodonUsers, mastodonUserContextOptions);
            return mastodonUsers;
        }

        public async Task<PagedList<MastodonUser>> GetFollowing(string followerUserID, MastodonUserContextOptions mastodonUserContextOptions = null, PagingOptions pagingOptions = null)
        {
            var effectivePagingOptions = pagingOptions ?? new PagingOptions();
            var mastodonClient = BuildMastodonApiClient();
            var mastodonUsersApiResult = await mastodonClient.GetAccountFollowing(followerUserID.ToLong(), effectivePagingOptions.MaxID.ToNullableLong(), effectivePagingOptions.SinceID.ToNullableLong(), effectivePagingOptions.Limit);
            var result = PagedList<MastodonUser>.Create(mastodonUsersApiResult, u => u.ToMastodonUser());
            await AddContextToMastodonUsers(result.Elements, mastodonUserContextOptions);
            return result;
        }

        public async Task<PagedList<MastodonUser>> GetFollowers(string followingUserID, MastodonUserContextOptions mastodonUserContextOptions = null, PagingOptions pagingOptions = null)
        {
            var effectivePagingOptions = pagingOptions ?? new PagingOptions();
            var mastodonClient = BuildMastodonApiClient();
            var mastodonUsersApiResult = await mastodonClient.GetAccountFollowers(followingUserID.ToLong(), effectivePagingOptions.MaxID.ToNullableLong(), effectivePagingOptions.SinceID.ToNullableLong(), effectivePagingOptions.Limit);
            var result = PagedList<MastodonUser>.Create(mastodonUsersApiResult, u => u.ToMastodonUser());
            await AddContextToMastodonUsers(result.Elements, mastodonUserContextOptions);
            return result;
        }

        public async Task AddContextToMastodonUsers(IEnumerable<MastodonUser> mastodonUsers, MastodonUserContextOptions mastodonUserContextOptions = null)
        {
            var effectiveMastodonUserContext = mastodonUserContextOptions ?? new MastodonUserContextOptions();
            var mastodonClient = BuildMastodonApiClient();

            foreach (var mastodonUser in mastodonUsers)
            {
                if (effectiveMastodonUserContext.IncludeFollowers)
                {
                    // Get the follower of this user
                    mastodonUser.Followers = (await mastodonClient.GetAccountFollowers(mastodonUser.MastodonUserId.ToLong()))
                        .Select(u => u.ToMastodonUser().Then(mu => mu.FollowsRelevantUser = true)).ToList();
                    mastodonUser.IsFollowedByActiveUser = mastodonUser.IsFollowedBy(UserMastodonConnectionDetails.MastodonUserID);
                }
                if (effectiveMastodonUserContext.IncludeFollowing)
                {
                    // Get the users this user us following
                    mastodonUser.Following = (await mastodonClient.GetAccountFollowing(mastodonUser.MastodonUserId.ToLong()))
                        .Select(u => u.ToMastodonUser().Then(mu => mu.IsFollowedByRelevantUser = true)).ToList();
                    mastodonUser.FollowsActiveUser = mastodonUser.Follows(UserMastodonConnectionDetails.MastodonUserID);
                }
            }

            if ((effectiveMastodonUserContext.IncludeIsFollowedByActiveUser || effectiveMastodonUserContext.IncludeFollowsActiveUser) && (!effectiveMastodonUserContext.IncludeFollowers || !effectiveMastodonUserContext.IncludeFollowing))
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
        }

        public async Task AddContextToMastodonUser(MastodonUser mastodonUser, MastodonUserContextOptions mastodonUserContextOptions = null)
        {
            var effectiveMastodonUserContext = mastodonUserContextOptions ?? new MastodonUserContextOptions();
            var mastodonClient = BuildMastodonApiClient();

            if (effectiveMastodonUserContext.IncludeFollowers)
            {
                // Get the follower of this user
                mastodonUser.Followers = (await mastodonClient.GetAccountFollowers(mastodonUser.MastodonUserId.ToLong()))
                    .Select(u => u.ToMastodonUser().Then(mu => mu.FollowsRelevantUser = true)).ToList();
                mastodonUser.IsFollowedByActiveUser = mastodonUser.Followers.Any(f => f.MastodonUserId == UserMastodonConnectionDetails.MastodonUserID);
            }
            if (effectiveMastodonUserContext.IncludeFollowing)
            {
                // Get the users this user us following
                mastodonUser.Following = (await mastodonClient.GetAccountFollowing(mastodonUser.MastodonUserId.ToLong()))
                    .Select(u => u.ToMastodonUser().Then(mu => mu.IsFollowedByRelevantUser = true)).ToList();
                mastodonUser.FollowsActiveUser = mastodonUser.Following.Any(f => f.MastodonUserId == UserMastodonConnectionDetails.MastodonUserID);
            }

            if ((effectiveMastodonUserContext.IncludeIsFollowedByActiveUser || effectiveMastodonUserContext.IncludeFollowsActiveUser) && (!mastodonUser.IsFollowedByActiveUser.HasValue || !mastodonUser.FollowsActiveUser.HasValue))
            {
                // We haven't gotten any follow data, but we still need to know if this user has a relationship with the active user
                var userRelationships = (await mastodonClient.GetAccountRelationships(mastodonUser.MastodonUserId.ToLong())).ToArray();
                mastodonUser.FollowsActiveUser = userRelationships.Length == 1 ? userRelationships[0].FollowedBy : false;
                mastodonUser.IsFollowedByActiveUser = userRelationships.Length == 1 ? userRelationships[0].Following : false;
            }
        }

        public async Task<MastodonRelationship> Follow(string userID, bool followUser)
        {
            if (followUser)
            {
                return (await BuildMastodonApiClient().Follow(userID.ToLong())).ToMastodonRelationship();
            }
            else
            {
                return (await BuildMastodonApiClient().Unfollow(userID.ToLong())).ToMastodonRelationship();
            }
        }

        /// <summary>
        /// Updates the currently authenticated user's profile information
        /// </summary>
        /// <param name="display_name"></param>
        /// <param name="bio"></param>
        /// <param name="avatar"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public async Task<MastodonUser> updateMastodonProfile(string display_name, string bio, string avatar, string header)
        {
            System.Diagnostics.Debug.WriteLine(avatar);
            return (await BuildMastodonApiClient().UpdateCredentials(display_name, bio, avatar, header)).ToMastodonUser();
        }

        #endregion User

        #region Posts

        public async Task<MastodonAttachment> UploadAttachment(Stream attachment)
        {
            return (await BuildMastodonApiClient().UploadMedia(attachment)).ToMastodonAttachment();
        }

        /// <summary>
        /// Reposts or un-reposts a post
        /// </summary>
        /// <param name="postID"></param>
        /// <param name="rePost"></param>
        /// <returns></returns>
        public async Task<MastodonPost> Repost(string postID, bool repost)
        {
            if (repost)
            {
                return (await BuildMastodonApiClient().Reblog(postID.ToLong())).ToPost();
            }
            else
            {
                return (await BuildMastodonApiClient().Unreblog(postID.ToLong())).ToPost();
            }
        }

        /// <summary>
        /// Likes or unlikes a post
        /// </summary>
        /// <param name="postID"></param>
        /// <param name="like"></param>
        /// <returns></returns>
        public async Task<MastodonPost> Like(string postID, bool like)
        {
            if (like)
            {
                return (await BuildMastodonApiClient().Favourite(postID.ToLong())).ToPost();
            }
            else
            {
                return (await BuildMastodonApiClient().Unfavourite(postID.ToLong())).ToPost();
            }
        }

        public async Task AddContextToMastodonPosts(IEnumerable<MastodonPost> mastodonPosts, MastodonPostContextOptions mastodonPostContextOptions = null)
        {
            foreach (var mastodonPost in mastodonPosts)
            {
                await AddContextToMastodonPost(mastodonPost, mastodonPostContextOptions);
            }
        }

        public async Task AddContextToMastodonPost(MastodonPost mastodonPost, MastodonPostContextOptions mastodonPostContextOptions = null)
        {
            var effectiveMastodonPostContextOptions = mastodonPostContextOptions ?? new MastodonPostContextOptions();
            var mastodonClient = BuildMastodonApiClient();

            if (effectiveMastodonPostContextOptions.IncludeAncestors || effectiveMastodonPostContextOptions.IncludeDescendants)
            {
                var statusContext = await mastodonClient.GetStatusContext(mastodonPost.Id.ToLong());
                if (effectiveMastodonPostContextOptions.IncludeAncestors)
                {
                    mastodonPost.Ancestors = statusContext.Ancestors.Select(s => s.ToPost()).ToList();
                }
                if (effectiveMastodonPostContextOptions.IncludeDescendants)
                {
                    mastodonPost.Descendants = statusContext.Descendants.Select(s => s.ToPost()).ToList();
                }
            }
        }

        public async Task<MastodonPost> GetPost(string postID, MastodonPostContextOptions mastodonPostContextOptions = null)
        {
            var mastodonClient = BuildMastodonApiClient();
            var post = (await mastodonClient.GetStatus(postID.ToLong())).ToPost();
            await AddContextToMastodonPost(post, mastodonPostContextOptions);
            return post;
        }

        public async Task<PagedList<MastodonPost>> GetPostsByAuthorUserID(string authorMastodonUserID, MastodonPostContextOptions mastodonPostContextOptions = null, PagingOptions pagingOptions = null)
        {
            var effectivePagingOptions = pagingOptions ?? new PagingOptions();
            var mastodonClient = BuildMastodonApiClient();
            var mastodonPostsApiResult = await mastodonClient.GetAccountStatuses(authorMastodonUserID.ToLong(), effectivePagingOptions.MaxID.ToNullableLong(), effectivePagingOptions.SinceID.ToNullableLong(), effectivePagingOptions.Limit, false, false);
            var result = PagedList<MastodonPost>.Create(mastodonPostsApiResult, s => s.ToPost());
            await AddContextToMastodonPosts(result.Elements, mastodonPostContextOptions);
            return result;
        }

        public async Task<PagedList<MastodonPost>> GetPostsByHashTag(string hashTag, MastodonPostContextOptions mastodonPostContextOptions = null, PagingOptions pagingOptions = null)
        {
            var effectivePagingOptions = pagingOptions ?? new PagingOptions();
            var mastodonClient = BuildMastodonApiClient();
            var mastodonPostsApiResult = await mastodonClient.GetTagTimeline(hashTag, effectivePagingOptions.MaxID.ToNullableLong(), effectivePagingOptions.SinceID.ToNullableLong(), effectivePagingOptions.Limit);
            var result = PagedList<MastodonPost>.Create(mastodonPostsApiResult, s => s.ToPost());
            await AddContextToMastodonPosts(result.Elements, mastodonPostContextOptions);
            return result;
        }

        public async Task<PagedList<MastodonPost>> GetPostsOnActiveUserTimeline(MastodonPostContextOptions mastodonPostContextOptions = null, PagingOptions pagingOptions = null)
        {
            var effectivePagingOptions = pagingOptions ?? new PagingOptions();
            var mastodonClient = BuildMastodonApiClient();
            var mastodonPostsApiResult = await mastodonClient.GetHomeTimeline(effectivePagingOptions.MaxID.ToNullableLong(), effectivePagingOptions.SinceID.ToNullableLong(), effectivePagingOptions.Limit);
            var result = PagedList<MastodonPost>.Create(mastodonPostsApiResult, s => s.ToPost());
            await AddContextToMastodonPosts(result.Elements, mastodonPostContextOptions);
            return result;
        }

        public async Task<MastodonPost> CreateNewPost(string message, MastodonPostVisibility visibility, string replyStatusId = null, IEnumerable<string> mediaIds = null, bool sensitive = false, string spoilerText = null)
        {
            var mastodonClient = BuildMastodonApiClient();
            return (await mastodonClient.PostStatus(message, visibility.ToVisibility(), replyStatusId.ToNullableLong(), mediaIds.ToLongs(), sensitive, spoilerText)).ToPost();
        }

        #endregion Posts
    }
}
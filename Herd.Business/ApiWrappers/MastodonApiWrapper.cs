﻿using Herd.Business.ApiWrappers.MastodonObjectContextOptions;
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
        IAuthenticationClient _authClient;
        IMastodonClient _mastodonClient;
        
        #region Public properties

        public string MastodonHostInstance { get; set; }
        public Registration AppRegistration { get; set; }
        public UserMastodonConnectionDetails UserMastodonConnectionDetails { get; set; }
        public const Scope ALL_SCOPES = Scope.Read | Scope.Write | Scope.Follow;

        #endregion Public properties

        #region Constructors

        public MastodonApiWrapper(IAuthenticationClient authClient = null, IMastodonClient mastodonClient = null)
            : this(null as string, authClient, mastodonClient) { }

        public MastodonApiWrapper(string mastodonHostInstance, IAuthenticationClient authClient = null, IMastodonClient mastodonClient = null)
            : this(null as Registration, authClient, mastodonClient)
        {
            MastodonHostInstance = mastodonHostInstance;
        }

        public MastodonApiWrapper(Registration registration, IAuthenticationClient authClient = null, IMastodonClient mastodonClient = null)
            : this(registration, null, authClient, mastodonClient)
        {
        }

        public MastodonApiWrapper(Registration registration, UserMastodonConnectionDetails userMastodonConnectionDetails, IAuthenticationClient authClient = null, IMastodonClient mastodonClient = null)
        {
            AppRegistration = registration;
            MastodonHostInstance = AppRegistration?.Instance;
            UserMastodonConnectionDetails = userMastodonConnectionDetails;
            _authClient = authClient;
            _mastodonClient = mastodonClient;
        }

        #endregion Constructors

        #region Private helper

        private IMastodonClient GetOrCreateMastodonClient()
        {
            if (_mastodonClient == null)
            {
                if (AppRegistration == null)
                {
                    throw new ArgumentNullException(nameof(AppRegistration));
                }
                if (UserMastodonConnectionDetails == null)
                {
                    throw new ArgumentNullException(nameof(UserMastodonConnectionDetails));
                }
                _mastodonClient = new MastodonClient(AppRegistration.ToMastodonAppRegistration(), UserMastodonConnectionDetails.ToMastodonAuth());
            }

            return _mastodonClient;
        }

        private IAuthenticationClient BuildMastodonAuthenticationClient()
        {
            if (_authClient == null)
            {
                if (string.IsNullOrWhiteSpace(MastodonHostInstance))
                {
                    throw new ArgumentException($"{nameof(MastodonHostInstance)} cannot be null or empty");
                }
                _authClient = AppRegistration == null
                    ? new AuthenticationClient(MastodonHostInstance)
                    : new AuthenticationClient(AppRegistration.ToMastodonAppRegistration());
            }

            return _authClient;
        }

        #endregion Private helper

        #region Auth

        #region Auth - Public methods

        public async Task<Registration> RegisterApp() => (await BuildMastodonAuthenticationClient().CreateApp("Herd", ALL_SCOPES)).ToHerdAppRegistration();

        public string GetOAuthUrl(string redirectURL = null) => BuildMastodonAuthenticationClient().OAuthUrl(redirectURL);

        public async Task<UserMastodonConnectionDetails> Connect(string token)
        {
            UserMastodonConnectionDetails = (await BuildMastodonAuthenticationClient().ConnectWithCode(token)).ToHerdConnectionDetails(AppRegistration.ID, "-1");
            UserMastodonConnectionDetails.MastodonUserID = (await GetOrCreateMastodonClient().GetCurrentUser()).Id.ToString();
            return UserMastodonConnectionDetails;
        }

        #endregion Auth - Public methods

        #endregion Auth

        #region User

        public async Task<MastodonUser> GetActiveUserMastodonAccount(MastodonUserContextOptions mastodonUserContextOptions = null)
        {
            var mastodonClient = GetOrCreateMastodonClient();
            var mastodonUser = (await mastodonClient.GetCurrentUser()).ToMastodonUser();
            await AddContextToMastodonUser(mastodonUser, mastodonUserContextOptions);
            return mastodonUser;
        }

        public async Task<MastodonUser> GetMastodonAccount(string id, MastodonUserContextOptions mastodonUserContextOptions = null)
        {
            var mastodonClient = GetOrCreateMastodonClient();
            var mastodonUser = (await mastodonClient.GetAccount(id.ToLong())).ToMastodonUser();
            await AddContextToMastodonUser(mastodonUser, mastodonUserContextOptions);
            return mastodonUser;
        }

        public async Task<IList<MastodonUser>> GetUsersByName(string name, MastodonUserContextOptions mastodonUserContextOptions = null, PagingOptions pagingOptions = null)
        {
            var effectivePagingOptions = pagingOptions ?? new PagingOptions();
            var mastodonClient = GetOrCreateMastodonClient();
            var mastodonUsersApiTask = mastodonClient.SearchAccounts(name, effectivePagingOptions.Limit);
            var mastodonUsers = (await mastodonUsersApiTask).Select(u => u.ToMastodonUser()).ToList();
            await AddContextToMastodonUsers(mastodonUsers, mastodonUserContextOptions);
            return mastodonUsers;
        }

        public async Task<PagedList<MastodonUser>> GetFollowing(string followerUserID, MastodonUserContextOptions mastodonUserContextOptions = null, PagingOptions pagingOptions = null)
        {
            var effectivePagingOptions = pagingOptions ?? new PagingOptions();
            var mastodonClient = GetOrCreateMastodonClient();
            var mastodonUsersApiResult = await mastodonClient.GetAccountFollowing(followerUserID.ToLong(), effectivePagingOptions.MaxID.ToNullableLong(), effectivePagingOptions.SinceID.ToNullableLong(), effectivePagingOptions.Limit);
            var result = PagedList<MastodonUser>.Create(mastodonUsersApiResult, u => u.ToMastodonUser());
            await AddContextToMastodonUsers(result.Elements, mastodonUserContextOptions);
            return result;
        }

        public async Task<PagedList<MastodonUser>> GetFollowers(string followingUserID, MastodonUserContextOptions mastodonUserContextOptions = null, PagingOptions pagingOptions = null)
        {
            var effectivePagingOptions = pagingOptions ?? new PagingOptions();
            var mastodonClient = GetOrCreateMastodonClient();
            var mastodonUsersApiResult = await mastodonClient.GetAccountFollowers(followingUserID.ToLong(), effectivePagingOptions.MaxID.ToNullableLong(), effectivePagingOptions.SinceID.ToNullableLong(), effectivePagingOptions.Limit);
            var result = PagedList<MastodonUser>.Create(mastodonUsersApiResult, u => u.ToMastodonUser());
            await AddContextToMastodonUsers(result.Elements, mastodonUserContextOptions);
            return result;
        }

        public async Task AddContextToMastodonUsers(IEnumerable<MastodonUser> mastodonUsers, MastodonUserContextOptions mastodonUserContextOptions = null)
        {
            var effectiveMastodonUserContext = mastodonUserContextOptions ?? new MastodonUserContextOptions();
            var mastodonClient = GetOrCreateMastodonClient();

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
            var mastodonClient = GetOrCreateMastodonClient();

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
                return (await GetOrCreateMastodonClient().Follow(userID.ToLong())).ToMastodonRelationship();
            }
            else
            {
                return (await GetOrCreateMastodonClient().Unfollow(userID.ToLong())).ToMastodonRelationship();
            }
        }

        /// <summary>
        /// Updates the mastodon profile information
        /// </summary>
        /// <param name="display_name"></param>
        /// <param name="bio"></param>
        /// <param name="avatarImage"></param>
        /// <param name="headerImage"></param>
        /// <returns></returns>
        public async Task<MastodonUser> UpdateMastodonProfile(string display_name, string bio, Stream avatarImage, Stream headerImage)
        {
            var apiTask = GetOrCreateMastodonClient().UpdateCredentials
            (
                display_name,
                bio,
                avatarImage == null ? null : new MediaDefinition(avatarImage, Guid.NewGuid().ToString()),
                headerImage == null ? null : new MediaDefinition(headerImage, Guid.NewGuid().ToString())
            );
            return (await apiTask).ToMastodonUser();
        }



        #endregion User

        #region Posts

        public async Task<MastodonAttachment> UploadAttachment(Stream attachment)
        {
            return (await GetOrCreateMastodonClient().UploadMedia(attachment)).ToMastodonAttachment();
        }

        /// <summary>
        /// Likes or unlikes a post
        /// </summary>
        /// <param name="postID"></param>
        /// <returns></returns>
        public async Task DeletePost(string postID)
        {
           await GetOrCreateMastodonClient().DeleteStatus(postID.ToLong());
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
                return (await GetOrCreateMastodonClient().Reblog(postID.ToLong())).ToPost();
            }
            else
            {
                return (await GetOrCreateMastodonClient().Unreblog(postID.ToLong())).ToPost();
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
                return (await GetOrCreateMastodonClient().Favourite(postID.ToLong())).ToPost();
            }
            else
            {
                return (await GetOrCreateMastodonClient().Unfavourite(postID.ToLong())).ToPost();
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
            var mastodonClient = GetOrCreateMastodonClient();

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
            var mastodonClient = GetOrCreateMastodonClient();
            var post = (await mastodonClient.GetStatus(postID.ToLong())).ToPost();
            await AddContextToMastodonPost(post, mastodonPostContextOptions);
            return post;
        }

        public async Task<PagedList<MastodonPost>> GetPostsByAuthorUserID(string authorMastodonUserID, MastodonPostContextOptions mastodonPostContextOptions = null, PagingOptions pagingOptions = null)
        {
            var effectivePagingOptions = pagingOptions ?? new PagingOptions();
            var mastodonClient = GetOrCreateMastodonClient();
            var mastodonPostsApiResult = await mastodonClient.GetAccountStatuses(authorMastodonUserID.ToLong(), effectivePagingOptions.MaxID.ToNullableLong(), effectivePagingOptions.SinceID.ToNullableLong(), effectivePagingOptions.Limit, false, false);
            var result = PagedList<MastodonPost>.Create(mastodonPostsApiResult, s => s.ToPost());
            await AddContextToMastodonPosts(result.Elements, mastodonPostContextOptions);
            return result;
        }

        public async Task<PagedList<MastodonPost>> GetPostsByHashTag(string hashTag, MastodonPostContextOptions mastodonPostContextOptions = null, PagingOptions pagingOptions = null)
        {
            var effectivePagingOptions = pagingOptions ?? new PagingOptions();
            var mastodonClient = GetOrCreateMastodonClient();
            var mastodonPostsApiResult = await mastodonClient.GetTagTimeline(hashTag, effectivePagingOptions.MaxID.ToNullableLong(), effectivePagingOptions.SinceID.ToNullableLong(), effectivePagingOptions.Limit);
            var result = PagedList<MastodonPost>.Create(mastodonPostsApiResult, s => s.ToPost());
            await AddContextToMastodonPosts(result.Elements, mastodonPostContextOptions);
            return result;
        }

        public async Task<PagedList<MastodonPost>> GetPostsOnActiveUserTimeline(MastodonPostContextOptions mastodonPostContextOptions = null, PagingOptions pagingOptions = null)
        {
            var effectivePagingOptions = pagingOptions ?? new PagingOptions();
            var mastodonClient = GetOrCreateMastodonClient();
            var mastodonPostsApiResult = await mastodonClient.GetHomeTimeline(effectivePagingOptions.MaxID.ToNullableLong(), effectivePagingOptions.SinceID.ToNullableLong(), effectivePagingOptions.Limit);
            var result = PagedList<MastodonPost>.Create(mastodonPostsApiResult, s => s.ToPost());
            await AddContextToMastodonPosts(result.Elements, mastodonPostContextOptions);
            return result;
        }

        public async Task<PagedList<MastodonPost>> GetPostsOnPublicTimeline(MastodonPostContextOptions mastodonPostContextOptions = null, PagingOptions pagingOptions = null)
        {
            var effectivePagingOptions = pagingOptions ?? new PagingOptions();
            var mastodonClient = GetOrCreateMastodonClient();
            var mastodonPostsApiResult = await mastodonClient.GetPublicTimeline(effectivePagingOptions.MaxID.ToNullableLong(), effectivePagingOptions.SinceID.ToNullableLong(), effectivePagingOptions.Limit, true);
            var result = PagedList<MastodonPost>.Create(mastodonPostsApiResult, s => s.ToPost());
            await AddContextToMastodonPosts(result.Elements, mastodonPostContextOptions);
            return result;
        }

        public async Task<MastodonPost> CreateNewPost(string message, MastodonPostVisibility visibility, string replyStatusId = null, IEnumerable<string> mediaIds = null, bool sensitive = false, string spoilerText = null)
        {
            var mastodonClient = GetOrCreateMastodonClient();
            return (await mastodonClient.PostStatus(message, visibility.ToVisibility(), replyStatusId.ToNullableLong(), mediaIds.ToLongs(), sensitive, spoilerText)).ToPost();
        }

        #endregion Posts

        #region Notifications

        /// <summary>
        /// Goes through each notification in a list of MastodonNotifications and adds context to each Post,
        /// if the notification has a Post
        /// </summary>
        /// <param name="mastodonNotifications"></param>
        /// <param name="mastodonPostContextOptions"></param>
        /// <returns></returns>
        public async Task AddContextToMastodonNotificationsPosts(IEnumerable<MastodonNotification> mastodonNotifications, MastodonPostContextOptions mastodonPostContextOptions = null)
        {
            foreach (var mastodonNotification in mastodonNotifications)
            {
                if (mastodonNotification.Status != null)
                {
                    await AddContextToMastodonPost(mastodonNotification.Status, mastodonPostContextOptions);
                }
            }
        }

        /// <summary>
        /// Gets the lastest notifications for the active Mastodon user
        /// </summary>
        /// <param name="mastodonPostContextOptions"></param>
        /// <param name="pagingOptions"></param>
        /// <returns></returns>
        public async Task<PagedList<MastodonNotification>> GetActiveUserNotifications(MastodonPostContextOptions mastodonPostContextOptions = null, PagingOptions pagingOptions = null)
        {
            var effectivePagingOptions = pagingOptions ?? new PagingOptions();
            var mastodonClient = GetOrCreateMastodonClient();
            var mastodonNotificationsApiResult = await mastodonClient.GetNotifications(effectivePagingOptions.MaxID.ToNullableLong(), effectivePagingOptions.SinceID.ToNullableLong(), effectivePagingOptions.Limit);
            var result = PagedList<MastodonNotification>.Create(mastodonNotificationsApiResult, s => s.ToMastodonNotification());
            await AddContextToMastodonNotificationsPosts(result.Elements, mastodonPostContextOptions);
            return result;
        }

        #endregion Notifications
    }
}
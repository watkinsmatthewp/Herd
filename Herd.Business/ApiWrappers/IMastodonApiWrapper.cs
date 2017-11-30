using Herd.Business.ApiWrappers.MastodonObjectContextOptions;
using Herd.Business.Models;
using Herd.Business.Models.Commands;
using Herd.Business.Models.Entities;
using Herd.Data.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Herd.Business.ApiWrappers
{
    public interface IMastodonApiWrapper
    {
        #region Public properties

        string MastodonHostInstance { get; set; }
        Registration AppRegistration { get; set; }
        UserMastodonConnectionDetails UserMastodonConnectionDetails { get; set; }

        #endregion Public properties

        #region Auth

        Task<Registration> RegisterApp();

        string GetOAuthUrl(string redirectURL);

        Task<UserMastodonConnectionDetails> Connect(string token);

        #endregion Auth

        #region User

        Task AddContextToMastodonUsers(IEnumerable<MastodonUser> mastodonUsers, MastodonUserContextOptions mastodonUserContextOptions = null);

        Task AddContextToMastodonUser(MastodonUser mastodonUser, MastodonUserContextOptions mastodonUserContextOptions = null);

        Task<IList<MastodonUser>> GetUsersByName(string name, MastodonUserContextOptions mastodonUserContextOptions = null, PagingOptions pagingOptions = null);

        Task<MastodonUser> GetActiveUserMastodonAccount(MastodonUserContextOptions mastodonUserContextOptions = null);

        Task<MastodonUser> GetMastodonAccount(string id, MastodonUserContextOptions mastodonUserContextOptions = null);

        Task<PagedList<MastodonUser>> GetFollowing(string followerUserID, MastodonUserContextOptions mastodonUserContextOptions = null, PagingOptions pagingOptions = null);

        Task<PagedList<MastodonUser>> GetFollowers(string followingUserID, MastodonUserContextOptions mastodonUserContextOptions = null, PagingOptions pagingOptions = null);

        Task<MastodonRelationship> Follow(string userID, bool followUser);

        Task<MastodonUser> UpdateMastodonProfile(string display_name, string bio, Stream avatarImage, Stream headerImage);

        #endregion User

        #region Posts

        Task<MastodonPost> Repost(string postID, bool repost);

        Task<MastodonPost> Like(string postID, bool like);

        Task DeletePost(string postID);

        Task AddContextToMastodonPosts(IEnumerable<MastodonPost> mastodonPosts, MastodonPostContextOptions mastodonPostContextOptions = null);

        Task AddContextToMastodonPost(MastodonPost mastodonPost, MastodonPostContextOptions mastodonPostContextOptions = null);

        Task<MastodonPost> GetPost(string postID, MastodonPostContextOptions mastodonPostContextOptions = null);

        Task<PagedList<MastodonPost>> GetPostsByAuthorUserID(string authorMastodonUserID, MastodonPostContextOptions mastodonPostContextOptions = null, PagingOptions pagingOptions = null);

        Task<PagedList<MastodonPost>> GetPostsByHashTag(string hashTag, MastodonPostContextOptions mastodonPostContextOptions = null, PagingOptions pagingOptions = null);

        Task<PagedList<MastodonPost>> GetPostsOnActiveUserTimeline(MastodonPostContextOptions mastodonPostContextOptions = null, PagingOptions pagingOptions = null);

        Task<PagedList<MastodonPost>> GetPostsOnPublicTimeline(MastodonPostContextOptions mastodonPostContextOptions = null, PagingOptions pagingOptions = null);

        Task<MastodonPost> CreateNewPost(string message, MastodonPostVisibility visibility, string replyStatusId = null, IEnumerable<string> mediaIds = null, bool sensitive = false, string spoilerText = null);

        Task<MastodonAttachment> UploadAttachment(Stream attachment);
        #endregion Posts

        #region Notifications

        Task<PagedList<MastodonNotification>> GetActiveUserNotifications(MastodonPostContextOptions mastodonPostContextOptions = null, PagingOptions pagingOptions = null);

        Task AddContextToMastodonNotificationsPosts(IEnumerable<MastodonNotification> mastodonNotifications, MastodonPostContextOptions mastodonPostContextOptions = null);

        #endregion Notifications

    }
}
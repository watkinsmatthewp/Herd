using Herd.Business.Models.Entities;
using Herd.Business.ApiWrappers;
using Herd.Data.Models;
using Mastonet;
using Mastonet.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Herd.Business
{
    public static class ConversionExtensions
    {
        #region Auth

        public static Registration ToHerdAppRegistration(this AppRegistration mastodonAppRegistration) => new Registration
        {
            ClientId = mastodonAppRegistration.ClientId,
            ClientSecret = mastodonAppRegistration.ClientSecret,
            MastodonAppRegistrationID = mastodonAppRegistration.Id,
            Instance = mastodonAppRegistration.Instance
        };

        public static AppRegistration ToMastodonAppRegistration(this Registration herdAppRegistration) => new AppRegistration
        {
            ClientId = herdAppRegistration.ClientId,
            Id = herdAppRegistration.MastodonAppRegistrationID,
            ClientSecret = herdAppRegistration.ClientSecret,
            Instance = herdAppRegistration.Instance,
            Scope = MastodonApiWrapper.ALL_SCOPES
        };

        public static Auth ToMastodonAuth(this UserMastodonConnectionDetails connectionDetails) => new Auth
        {
            AccessToken = connectionDetails.ApiAccessToken,
            CreatedAt = connectionDetails.CreatedAt,
            Scope = connectionDetails.Scope,
            TokenType = connectionDetails.TokenType
        };

        public static UserMastodonConnectionDetails ToHerdConnectionDetails(this Auth auth, long appRegistrationID, int mastodonUserID) => new UserMastodonConnectionDetails
        {
            ApiAccessToken = auth.AccessToken,
            AppRegistrationID = appRegistrationID,
            CreatedAt = auth.CreatedAt,
            Scope = auth.Scope,
            TokenType = auth.TokenType,
            MastodonUserID = mastodonUserID
        };

        #endregion Auth

        #region User

        public static MastodonUser ToMastodonUser(this Account account) => new MastodonUser
        {
            MastodonDisplayName = account.DisplayName,
            MastodonHeaderImageURL = account.HeaderUrl,
            MastodonProfileImageURL = account.AvatarUrl,
            MastodonShortBio = account.Note,
            MastodonUserId = account.Id,
            MastodonUserName = account.UserName,
            FollowersCount = account.FollowersCount,
            FollowingCount = account.FollowingCount
        };

        #endregion User

        #region Posts

        public static MastodonPost ToPost(this Status status)
        {
            var post = new MastodonPost
            {
                Author = status.Account.ToMastodonUser(),
                Content = status.Content,
                CreatedOnUTC = status.CreatedAt,
                FavouritesCount = status.FavouritesCount,
                Id = status.Id,
                InReplyToPostId = status.InReplyToId,
                IsFavourited = status.Favourited,
                IsReblogged = status.Reblogged,
                IsSensitive = status.Sensitive,
                ReblogCount = status.ReblogCount,
                SpoilerText = status.SpoilerText,
                Visibility = status.Visibility.ToMastodonPostVisibility(),
            };

            return post;
        }

        public static MastodonPostVisibility ToMastodonPostVisibility(this Visibility visibility)
        {
            return (MastodonPostVisibility)visibility;
        }

        public static Visibility ToVisibility(this MastodonPostVisibility visibility)
        {
            return (Visibility)visibility;
        }

        #endregion Posts
    }
}
using Herd.Business.ApiWrappers;
using Herd.Business.Models.Entities;
using Herd.Data.Models;
using Mastonet;
using Mastonet.Entities;
using System;
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
            MastodonAppRegistrationID = mastodonAppRegistration.Id.ToString(),
            Instance = mastodonAppRegistration.Instance
        };

        public static AppRegistration ToMastodonAppRegistration(this Registration herdAppRegistration) => new AppRegistration
        {
            ClientId = herdAppRegistration.ClientId,
            Id = herdAppRegistration.MastodonAppRegistrationID.ToLong(),
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

        public static UserMastodonConnectionDetails ToHerdConnectionDetails(this Auth auth, int appRegistrationID, string mastodonUserID) => new UserMastodonConnectionDetails
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
            MastodonUserId = account.Id.ToString(),
            MastodonUserName = account.UserName,
            PostCount = account.StatusesCount,
            FollowersCount = account.FollowersCount,
            FollowingCount = account.FollowingCount
        };

        public static MastodonRelationship ToMastodonRelationship(this Relationship relationship) => new MastodonRelationship
        {
            ID = relationship.Id.ToString(),
            Following = relationship.Following,
            FollowedBy = relationship.FollowedBy,
            Blocking = relationship.Blocking,
            Muting = relationship.Muting,
            Requested = relationship.Requested,
            DomainBlocking = relationship.DomainBlocking,
        };

        public static Relationship ToRelationship(this MastodonRelationship relationship) => new Relationship
        {
            Id = relationship.ID.ToLong(),
            Following = relationship.Following,
            FollowedBy = relationship.FollowedBy,
            Blocking = relationship.Blocking,
            Muting = relationship.Muting,
            Requested = relationship.Requested,
            DomainBlocking = relationship.DomainBlocking,
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
                Id = status.Id.ToString(),
                InReplyToPostId = status.InReplyToId.ToString(),
                IsFavourited = status.Favourited,
                IsReblogged = status.Reblogged,
                IsSensitive = status.Sensitive,
                MediaAttachment = status.MediaAttachments.Count() > 0 ?status.MediaAttachments.ElementAt(0).Url : "",
                ReblogCount = status.ReblogCount,
                SpoilerText = status.SpoilerText,
                Visibility = status.Visibility.ToMastodonPostVisibility(),
            };

            return post;
        }

        public static MastodonAttachment ToMastodonAttachment(this Attachment attachment)
        {
            return new MastodonAttachment
            {
                Id = attachment.Id.ToString(),
                Type = attachment.Type,
                Url = attachment.Url,
                RemoteUrl = attachment.RemoteUrl,
                PreviewUrl = attachment.PreviewUrl,
                TextUrl = attachment.TextUrl
            };
        }

        public static MastodonPostVisibility ToMastodonPostVisibility(this Visibility visibility)
        {
            return (MastodonPostVisibility)visibility;
        }

        public static Visibility ToVisibility(this MastodonPostVisibility visibility)
        {
            return (Visibility)visibility;
        }

        #region Notifications

        public static MastodonNotification ToMastodonNotification(this Notification notification)
        {
            return new MastodonNotification
            {
                Id = notification.Id.ToString(),
                Type = notification.Type,
                CreatedAt = notification.CreatedAt,
                Account = notification.Account.ToMastodonUser(),
                Status = notification.Status?.ToPost()
            };
        }

        #endregion Notifications

        #endregion Posts

        #region General

        public static IEnumerable<long> ToLongs(this IEnumerable<string> sCollection) => sCollection?.Select(s => s.ToLong());

        public static IEnumerable<long?> ToNullableLongs(this IEnumerable<string> sCollection) => sCollection.Select(s => s.ToNullableLong());

        public static long ToLong(this string s) => long.Parse(s);

        public static long? ToNullableLong(this string s) => string.IsNullOrWhiteSpace(s) ? null as long? : s.ToLong();

        #endregion General
    }
}
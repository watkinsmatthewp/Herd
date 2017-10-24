using Herd.Business.Models.Entities;
using Herd.Data.Models;
using System;
using System.Linq;

namespace Herd.Business.Extensions
{
    public static class EntityExtensions
    {
        public static bool PasswordIs(this UserAccount user, string plainTextPassword) => user.Security.SaltedPassword == plainTextPassword.Hashed(user.Security.SaltKey);

        public static bool Follows(this MastodonUser mastodonUser, string mastodonUserID)
        {
            if (mastodonUser.Following == null)
            {
                throw new ArgumentNullException(nameof(mastodonUser.Following), "The list of users this user is following is null. Make sure to ask for following users when you query the API");
            }
            return mastodonUser.Following.Any(f => f.MastodonUserId == mastodonUserID);
        }

        public static bool IsFollowedBy(this MastodonUser mastodonUser, string mastodonUserID)
        {
            if (mastodonUser.Followers == null)
            {
                throw new ArgumentNullException(nameof(mastodonUser.Followers), "The list of users that follow this user is null. Make sure to ask for following users when you query the API");
            }
            return mastodonUser.Followers.Any(f => f.MastodonUserId == mastodonUserID);
        }
    }
}
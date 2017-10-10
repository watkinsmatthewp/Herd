using Herd.Core;
using Herd.Data.Models;
using Mastonet.Entities;
using System;
using System.Linq;

namespace Herd.Business
{
    public static class Extensions
    {
        public static bool PasswordIs(this IUserAccount user, string plainTextPassword) => user.Security.SaltedPassword == plainTextPassword.Hashed(user.Security.SaltKey);

        public static string Hashed(this string passwordPlainText, long saltKey)
        {
            var random = new Random((int)(saltKey % int.MaxValue));
            var mungedPassword = new string($"{passwordPlainText}{saltKey}".OrderBy(c => random.Next()).ToArray());
            return mungedPassword.Hashed();
        }

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

        public static UserMastodonConnectionDetails ToHerdConnectionDetails(this Auth auth, long appRegistrationID) => new UserMastodonConnectionDetails
        {
            ApiAccessToken = auth.AccessToken,
            AppRegistrationID = appRegistrationID,
            CreatedAt = auth.CreatedAt,
            Scope = auth.Scope,
            TokenType = auth.TokenType
        };
    }
}
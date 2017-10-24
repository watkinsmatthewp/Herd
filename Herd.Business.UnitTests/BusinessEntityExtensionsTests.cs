using Herd.Business.Extensions;
using Herd.Business.Models.Entities;
using Herd.Core;
using Herd.Data.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace Herd.Business.UnitTests
{
    public class BusinessEntityExtensionsTests
    {
        private static Random _saltGenerator = new Random(Guid.NewGuid().GetHashCode());

        [Fact]
        public void PasswordIsTest()
        {
            var saltKey = _saltGenerator.Next();

            // Create user
            UserAccount user = new UserAccount
            {
                Email = "herder@gmail.com",
                ID = 1,
                Security = new UserAccountSecurity
                {
                    SaltKey = saltKey,
                    SaltedPassword = "password".Hashed(saltKey)
                },
                MastodonConnection = new UserMastodonConnectionDetails
                {
                    AppRegistrationID = 3,
                    ApiAccessToken = "123",
                    CreatedAt = "10/22/2017",
                    Scope = "narrow",
                    TokenType = "Shiny",
                    MastodonUserID = "2234"
                }
            };

            Assert.True(user.PasswordIs("password"));
        }

        [Fact]
        public void FollowsTest()
        {
            // Create Mastodon User
            MastodonUser user = new MastodonUser
            {
                MastodonUserId = "12",
                Following = new List<MastodonUser>
                {
                    new MastodonUser
                    {
                        MastodonUserId = "55"
                    }
                }
            };

            // Perform test
            Assert.True(user.Follows("55"));

            // Make user with null Following
            MastodonUser userNullFollowing = new MastodonUser
            {
                MastodonUserId = "15"
            };

            // Perform null catch test
            bool passed = false;
            try
            {
                userNullFollowing.Follows("67");
            }
            catch
            {
                passed = true;
            }

            Assert.True(passed);
        }

        [Fact]
        public void IsFollowedByTest()
        {
            // Create Mastodon User
            MastodonUser user = new MastodonUser
            {
                MastodonUserId = "12",
                Followers = new List<MastodonUser>
                {
                    new MastodonUser
                    {
                        MastodonUserId = "55"
                    }
                }
            };

            // Perform test
            Assert.True(user.IsFollowedBy("55"));

            // Make user with null Following
            MastodonUser userNullFollowers = new MastodonUser
            {
                MastodonUserId = "15"
            };

            // Perform null catch test
            bool passed = false;
            try
            {
                userNullFollowers.IsFollowedBy("67");
            }
            catch
            {
                passed = true;
            }

            Assert.True(passed);
        }
    }
}
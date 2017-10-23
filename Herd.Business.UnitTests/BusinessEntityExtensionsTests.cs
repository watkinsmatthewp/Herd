using Herd.Business.ApiWrappers;
using Herd.Business.ApiWrappers.MastodonObjectContextOptions;
using Herd.Business.Extensions;
using Herd.Business.Models;
using Herd.Business.Models.Commands;
using Herd.Business.Models.Entities;
using Herd.Core;
using Herd.Data.Models;
using Herd.Data.Providers;
using Herd.Logging;
using Herd.UnitTestCore;
using Mastonet.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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


        }

    }
}

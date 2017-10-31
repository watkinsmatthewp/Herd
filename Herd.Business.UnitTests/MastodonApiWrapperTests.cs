using Herd.Business.ApiWrappers;
using Herd.Core;
using Herd.Data.Models;
using Mastonet;
using Mastonet.Entities;
using Pose;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Herd.Business.UnitTests
{
    public class MastodonApiWrapperTests
    {
        [Fact]
        public void MastodonHostInstanceTest()
        {
            var apiWrapper = new MastodonApiWrapper() { MastodonHostInstance = "mastodon.instance" };
            Assert.Equal("mastodon.instance", apiWrapper.MastodonHostInstance);
        }

        [Fact]
        public void AppRegistrationTest()
        {
            var expectedAppRegistration = new Registration();
            var apiWrapper = new MastodonApiWrapper { AppRegistration = expectedAppRegistration };
            Assert.Equal(expectedAppRegistration, apiWrapper.AppRegistration);
        }

        [Fact]
        public void UserMastodonConnectionDetailsTest()
        {
            var userMastodonConnectionDetails = new UserMastodonConnectionDetails();
            var apiWrapper = new MastodonApiWrapper { UserMastodonConnectionDetails = userMastodonConnectionDetails };
            Assert.Equal(userMastodonConnectionDetails, apiWrapper.UserMastodonConnectionDetails);
        }

        [Fact]
        public void RegistrerAppTest()
        {
            var objectToReturn = new AppRegistration
            {
                ClientId = "client-id",
                ClientSecret = "client-secret",
                Id = 1234,
                Instance = "mastodon.instance",
                RedirectUri = "http://redirect.uri",
                Scope = Scope.Follow
            };

            var mastonetShim = Shim.Replace(() => Is.A<AuthenticationClient>().CreateApp("mastodon.instance", MastodonApiWrapper.ALL_SCOPES, null, null))
                .With(() => Task.FromResult(objectToReturn));

            PoseContext.Isolate(() =>
            {
                var apiWrapper = new MastodonApiWrapper() { MastodonHostInstance = "mastodon.instance" };
                var appRegistration = apiWrapper.RegisterApp().Synchronously();
                Assert.Equal("client-id", appRegistration.ClientId);
            }, mastonetShim);
        }
    }
}

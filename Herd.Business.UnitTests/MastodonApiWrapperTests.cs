using Herd.Business.ApiWrappers;
using Herd.Core;
using Herd.Data.Models;
using Mastonet;
using Mastonet.Entities;
using Moq;
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
        Mock<IMastodonClient> _mockMastodonClient = new Mock<IMastodonClient>();
        Mock<IAuthenticationClient> _mockAuthClient = new Mock<IAuthenticationClient>();

        [Fact]
        public void MastodonHostInstanceTest()
        {
            var apiWrapper = new MastodonApiWrapper { MastodonHostInstance = "mastodon.instance" };
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
            _mockAuthClient.Setup(a => a.CreateApp(It.IsAny<string>(), It.IsAny<Scope>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new AppRegistration
                {
                    ClientId = "client-id",
                    ClientSecret = "client-secret",
                    Id = 1234,
                    Instance = "mastodon.instance",
                    RedirectUri = "http://redirect.uri",
                    Scope = Scope.Follow
                }));
            var apiWrapper = new MastodonApiWrapper("mastodon.instance", _mockAuthClient.Object);
            var appRegistration = apiWrapper.RegisterApp().Synchronously();
            Assert.Equal("client-id", appRegistration.ClientId);
        }
    }
}

using Herd.Business.Models.Commands;
using Herd.Data.Models;
using Herd.Data.Providers;
using Herd.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Herd.Business.UnitTests
{
    public class HerdAppTests
    {
        [Fact]
        public void GetRegistrationTest()
        {
            // Tell Moq to create an objects that implement the interfaces of the HerdApp dependencies
            var mockData = new Mock<IHerdDataProvider>();
            var mockMastodonApiWrapper = new Mock<IMastodonApiWrapper>();
            var mockLogger = new Mock<IHerdLogger>();

            // Tell moq that when that object's GetAppRegistration method is called for ID 3,
            // return this HerdAppRegistrationDataModel object with the following properties
            mockData.Setup(d => d.GetAppRegistration(3)).Returns(new HerdAppRegistrationDataModel
            {
                ID = 3,
                ClientId = "client-id",
                ClientSecret = "client-secret",
                Instance = "mastodon.instance",
                MastodonAppRegistrationID = 42
            });

            // Create the HerdApp using the mock objects
            var herdApp = new HerdApp(mockData.Object, mockMastodonApiWrapper.Object, mockLogger.Object);

            // Run the HerdApp command (should execute the mock)
            var result = herdApp.GetRegistration(new HerdAppGetRegistrationCommand { ID = 3 });

            // Make sure the GetAppRegistration was called once with id 3
            mockData.Verify(d => d.GetAppRegistration(3), Times.Once());

            // Verify the result
            Assert.True(result?.Success);
            Assert.Equal(3, result.Data?.Registration?.ID);
            Assert.Equal("client-id", result.Data?.Registration?.ClientId);
            Assert.Equal("client-secret", result.Data?.Registration?.ClientSecret);
            Assert.Equal("mastodon.instance", result.Data?.Registration?.Instance);
            Assert.Equal(42, result.Data?.Registration?.MastodonAppRegistrationID);
        }

        [Fact]
        public void GetOAuthURLTest()
        {

            // Tell Moq to create an objects that implement the interfaces of the HerdApp dependencies
            var mockData = new Mock<IHerdDataProvider>();
            var mockMastodonApiWrapper = new Mock<IMastodonApiWrapper>();
            var mockLogger = new Mock<IHerdLogger>();

            // Setup the mocks
            mockData.Setup(d => d.GetAppRegistration(3)).Returns(new HerdAppRegistrationDataModel
            {
                ID = 3,
                ClientId = "client-id",
                ClientSecret = "client-secret",
                Instance = "mastodon.instance",
                MastodonAppRegistrationID = 42
            });

            mockMastodonApiWrapper.Setup(p => p.GetOAuthUrl("https://SentURL")).Returns("https://ReturnedURL");

            // Create the HerdApp using the mock objects
            var herdApp = new HerdApp(mockData.Object, mockMastodonApiWrapper.Object, mockLogger.Object);

            // Perform the test
            
            var result = herdApp.GetOAuthURL(new HerdAppGetOAuthURLCommand { AppRegistrationID = 3, ReturnURL = "https://SentURL" });

            // Verify the result
            Assert.True(result?.Success);
            Assert.Equal("https://ReturnedURL", result.Data?.URL);

        }
    }
}


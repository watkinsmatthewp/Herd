using Herd.Business.ApiWrappers;
using Herd.Business.Models.Commands;
using Herd.Business.Models.Entities;
using Herd.Data.Models;
using Herd.Data.Providers;
using Herd.Logging;
using Mastonet.Entities;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Herd.Business.UnitTests
{
    public class HerdAppTests
    {
        [Fact]
        public void GetRegistrationTest()
        {
            // Tell Moq to create an objects that implement the interfaces of the HerdApp dependencies
            var mockData = new Mock<IDataProvider>();
            var mockMastodonApiWrapper = new Mock<IMastodonApiWrapper>();
            var mockLogger = new Mock<ILogger>();

            // Tell moq that when that object's GetAppRegistration method is called for ID 3,
            // return this HerdAppRegistrationDataModel object with the following properties
            mockData.Setup(d => d.GetAppRegistration(3)).Returns(new Registration
            {
                ID = 3,
                ClientId = "client-id",
                ClientSecret = "client-secret",
                Instance = "mastodon.instance",
                MastodonAppRegistrationID = "42"
            });

            // Create the HerdApp using the mock objects
            var herdApp = new HerdApp(mockData.Object, mockMastodonApiWrapper.Object, mockLogger.Object);

            // Run the HerdApp command (should execute the mock)
            var result = herdApp.GetRegistration(new GetRegistrationCommand { ID = 3 });

            // Make sure the GetAppRegistration was called once with id 3
            mockData.Verify(d => d.GetAppRegistration(3), Times.Once());

            // Verify the result
            Assert.True(result?.Success);
            Assert.Equal(3, result.Data?.Registration?.ID);
            Assert.Equal("client-id", result.Data?.Registration?.ClientId);
            Assert.Equal("client-secret", result.Data?.Registration?.ClientSecret);
            Assert.Equal("mastodon.instance", result.Data?.Registration?.Instance);
            Assert.Equal("42", result.Data?.Registration?.MastodonAppRegistrationID);
        }

        [Fact]
        public void GetOAuthURLTest()
        {
            // Tell Moq to create an objects that implement the interfaces of the HerdApp dependencies
            var mockData = new Mock<IDataProvider>();
            var mockMastodonApiWrapper = new Mock<IMastodonApiWrapper>();
            var mockLogger = new Mock<ILogger>();

            // Setup the mocks
            mockData.Setup(d => d.GetAppRegistration(3)).Returns(new Registration
            {
                ID = 3,
                ClientId = "client-id",
                ClientSecret = "client-secret",
                Instance = "mastodon.instance",
                MastodonAppRegistrationID = "42"
            });

            mockMastodonApiWrapper.Setup(p => p.GetOAuthUrl("https://SentURL")).Returns("https://ReturnedURL");

            // Create the HerdApp using the mock objects
            var herdApp = new HerdApp(mockData.Object, mockMastodonApiWrapper.Object, mockLogger.Object);

            // Perform the test
            var result = herdApp.GetOAuthURL(new GetOAuthURLCommand { AppRegistrationID = 3, ReturnURL = "https://SentURL" });

            // Verify the result
            Assert.True(result?.Success);
            Assert.Equal("https://ReturnedURL", result.Data?.URL);
        }

        [Fact]
        public void GetOrCreateRegistrationTest()
        {
            // Tell Moq to create an objects that implement the interfaces of the HerdApp dependencies
            var mockData = new Mock<IDataProvider>();
            var mockMastodonApiWrapper = new Mock<IMastodonApiWrapper>();
            var mockLogger = new Mock<ILogger>();

            mockData.Setup(d => d.GetAppRegistration("instance")).Returns(new Registration
            {
                ID = 3,
                ClientId = "client-id",
                ClientSecret = "client-secret",
                Instance = "mastodon.instance",
                MastodonAppRegistrationID = "42"
            });

            mockData.Setup(d => d.CreateAppRegistration(new Registration())).Returns(new Registration
            {
                ID = 3,
                ClientId = "client-id",
                ClientSecret = "client-secret",
                Instance = "mastodon.instance",
                MastodonAppRegistrationID = "42"
            });

            // Create the HerdApp using the mock objects
            var herdApp = new HerdApp(mockData.Object, mockMastodonApiWrapper.Object, mockLogger.Object);

            // Run the HerdApp command (should execute the mock)
            var result = herdApp.GetOrCreateRegistration(new GetOrCreateRegistrationCommand { Instance = "instance" });

            // Make sure the GetAppRegistration was called once with id 3
            mockData.Verify(d => d.GetAppRegistration("instance"), Times.Once());

            // Verify the result
            Assert.True(result?.Success);
            Assert.Equal(3, result.Data?.Registration?.ID);
            Assert.Equal("client-id", result.Data?.Registration?.ClientId);
            Assert.Equal("client-secret", result.Data?.Registration?.ClientSecret);
            Assert.Equal("mastodon.instance", result.Data?.Registration?.Instance);
            Assert.Equal("42", result.Data?.Registration?.MastodonAppRegistrationID);

            //TODO need to check if GetAppRegistration is null?
        }

        [Fact]
        public void GetRecentFeedItemsTest()
        {
            // Tell Moq to create an objects that implement the interfaces of the HerdApp dependencies
            var mockData = new Mock<IDataProvider>();
            var mockMastodonApiWrapper = new Mock<IMastodonApiWrapper>();
            var mockLogger = new Mock<ILogger>();

            Status status = new Status();
            status.Content = "Hello, World!";
            List<Status> list = new List<Status>();
            list.Add(status);

            mockMastodonApiWrapper.Setup(d => d.GetRecentPosts(false, false, null, null, 1)).Returns(Task.FromResult<List<MastodonPost>>(new List<MastodonPost>()));

            // Create the HerdApp using the mock objects
            var herdApp = new HerdApp(mockData.Object, mockMastodonApiWrapper.Object, mockLogger.Object);

            // Run the HerdApp command (should execute the mock)
            var result = herdApp.GetRecentFeedItems(new GetRecentPostsCommand { MaxCount = 1 });

            // Verify the result, should add more tests when dummy data is removed
            Assert.True(result?.Success);
        }

        [Fact]
        public void CreateNewPostTest()
        {
            // Tell Moq to create an objects that implement the interfaces of the HerdApp dependencies
            var mockData = new Mock<IDataProvider>();
            var mockMastodonApiWrapper = new Mock<IMastodonApiWrapper>();
            var mockLogger = new Mock<ILogger>();
            
            mockMastodonApiWrapper.Setup(d => d.CreateNewPost("Hello World!", MastodonPostVisibility.Public,
                null, null, false, null)).Returns(Task.FromResult(new MastodonPost()));

            // Create the HerdApp using the mock objects
            var herdApp = new HerdApp(mockData.Object, mockMastodonApiWrapper.Object, mockLogger.Object);

            // Run the HerdApp command (should execute the mock)
            var result = herdApp.CreateNewPost(new HerdAppCreateNewPostCommand { Message = "Hello, World." });

            // Verify the result, do we need to check any more than this?
            Assert.True(result?.Success);
        }

        [Fact]
        public void HerdAppFollowUserTest()
        {
            // Tell Moq to create an objects that implement the interfaces of the HerdApp dependencies
            var mockData = new Mock<IDataProvider>();
            var mockMastodonApiWrapper = new Mock<IMastodonApiWrapper>();
            var mockLogger = new Mock<ILogger>();

            // Mock the Task
            TaskCompletionSource<MastodonRelationship> taskCompletion = new TaskCompletionSource<MastodonRelationship>();
            taskCompletion.SetResult(new MastodonRelationship { ID = "1", Following = true });
            
            // Mock the Follow function
            mockMastodonApiWrapper.Setup(d => d.Follow("1", true)).Returns(taskCompletion.Task);
            mockMastodonApiWrapper.Setup(d => d.Follow("1", false)).Returns(taskCompletion.Task);

            var herdApp = new HerdApp(mockData.Object, mockMastodonApiWrapper.Object, mockLogger.Object);

            // Run the HerdApp command (should execute the mock)
            var result = herdApp.FollowUser(new FollowUserCommand { UserID = "1", FollowUser = true });

            // Verify the result
            Assert.True(result?.Success);
            // TODO DO more checks, should the function return more than just a CommandResult?
            //Assert.True(result.Data.Following);


            // Run the HerdApp command (should execute the mock)
            result = herdApp.FollowUser(new FollowUserCommand { UserID = "1", FollowUser = false });

            // Verify the result
            Assert.True(result?.Success);
        }

        
    }
}
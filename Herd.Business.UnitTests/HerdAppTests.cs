using Herd.Business.ApiWrappers;
using Herd.Business.ApiWrappers.MastodonObjectContextOptions;
using Herd.Business.Models;
using Herd.Business.Models.Commands;
using Herd.Business.Models.Entities;
using Herd.Core;
using Herd.Data.Models;
using Herd.Data.Providers;
using Herd.Logging;
using Herd.UnitTestCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Herd.Business.UnitTests
{
    public class HerdAppTests
    {
        private Mock<IDataProvider> _mockData = new Mock<IDataProvider>();
        private Mock<IMastodonApiWrapper> _mockMastodonApiWrapper = new Mock<IMastodonApiWrapper>();
        private Mock<ILogger> _mockLogger = new Mock<ILogger>();

        #region General

        [Fact]
        public void SystemErrorHandledCorrectlyTest()
        {
            var herdApp = new HerdApp(_mockData.Object, _mockMastodonApiWrapper.Object, _mockLogger.Object);
            var result = herdApp.SearchPosts(null);
            Assert.False(result?.Success);
            Assert.True(result.HasSystemErrors);
            Assert.False(result.HasUserErrors);
            Assert.Single(result.Errors);
            Assert.Empty(result.UserErrors);
            Assert.Single(result.SystemErrors);
            Assert.Contains(new NullReferenceException().Message, result.SystemErrors.First().Message);
        }

        #endregion General

        #region App Registration

        [Fact]
        public void GetRegistrationTest()
        {
            var expectedRegistration = new Registration
            {
                ID = 3,
                ClientId = "client-id",
                ClientSecret = "client-secret",
                Instance = "mastodon.instance",
                MastodonAppRegistrationID = "42"
            };

            _mockData.Setup(d => d.GetAppRegistration(3)).Returns(expectedRegistration);
            var herdApp = new HerdApp(_mockData.Object, _mockMastodonApiWrapper.Object, _mockLogger.Object);

            var result = herdApp.GetRegistration(new GetRegistrationCommand { ID = 3 });

            Assert.True(result?.Success);
            ExtendedAssert.ObjectsEqual(expectedRegistration, result.Data.Registration);
            _mockData.Verify(d => d.GetAppRegistration(3), Times.Once());
        }

        [Fact]
        public void GetOrCreateRegistrationWhenRegistrationDoesNotAlreadyExistTest()
        {
            _mockData.Setup(d => d.GetAppRegistration("mastodon.instance")).Returns(null as Registration);
            _mockMastodonApiWrapper.Setup(a => a.RegisterApp()).Returns(Task.FromResult(new Registration
            {
                ClientId = "client-id",
                ClientSecret = "client-secret",
                Instance = "mastodon.instance",
                MastodonAppRegistrationID = "42",
                ID = -1
            }));
            _mockData.Setup(d => d.CreateAppRegistration(It.Is<Registration>(r => r.Instance == "mastodon.instance")))
                .Returns<Registration>(registrationToCreate => registrationToCreate.With(r => r.ID = 3));

            var herdApp = new HerdApp(_mockData.Object, _mockMastodonApiWrapper.Object, _mockLogger.Object);

            var result = herdApp.GetOrCreateRegistration(new GetOrCreateRegistrationCommand { Instance = "mastodon.instance" });

            // Verify the result
            var expectedRegistration = new Registration
            {
                ID = 3,
                ClientId = "client-id",
                ClientSecret = "client-secret",
                Instance = "mastodon.instance",
                MastodonAppRegistrationID = "42"
            };

            Assert.True(result?.Success);
            ExtendedAssert.ObjectsEqual(expectedRegistration, result.Data.Registration);

            // Make sure the GetAppRegistration and CreateAppRegistration were each called exactly once
            _mockData.Verify(d => d.GetAppRegistration("mastodon.instance"), Times.Once());
            _mockMastodonApiWrapper.Verify(a => a.RegisterApp(), Times.Once());
            _mockData.Verify(d => d.CreateAppRegistration(It.Is<Registration>(r => r.Instance == "mastodon.instance")), Times.Once());
        }

        [Fact]
        public void GetOrCreateRegistrationWhenRegistrationAlreadyExistsTest()
        {
            var expectedRegistration = new Registration
            {
                ID = 3,
                ClientId = "client-id",
                ClientSecret = "client-secret",
                Instance = "mastodon.instance",
                MastodonAppRegistrationID = "42"
            };

            _mockData.Setup(d => d.GetAppRegistration("mastodon.instance")).Returns(expectedRegistration);
            var herdApp = new HerdApp(_mockData.Object, _mockMastodonApiWrapper.Object, _mockLogger.Object);

            var result = herdApp.GetOrCreateRegistration(new GetOrCreateRegistrationCommand { Instance = "mastodon.instance" });

            // Verify the result
            Assert.True(result?.Success);
            ExtendedAssert.ObjectsEqual(expectedRegistration, result.Data.Registration);

            // Make sure the GetAppRegistration was called once and CreateAppRegistration was never called
            _mockData.Verify(d => d.GetAppRegistration("mastodon.instance"), Times.Once());
            _mockData.Verify(d => d.CreateAppRegistration(It.Is<Registration>(r => true)), Times.Never());
        }

        [Fact]
        public void GetOAuthURLTest()
        {
            _mockData.Setup(d => d.GetAppRegistration(4)).Returns(new Registration
            {
                ID = 4,
                ClientId = "client-id",
                ClientSecret = "client-secret",
                Instance = "mastodon.instance",
                MastodonAppRegistrationID = "42"
            });
            _mockMastodonApiWrapper.Setup(p => p.GetOAuthUrl("https://SentURL")).Returns("https://ReturnedURL");

            var herdApp = new HerdApp(_mockData.Object, _mockMastodonApiWrapper.Object, _mockLogger.Object);

            var result = herdApp.GetOAuthURL(new GetMastodonOAuthURLCommand { AppRegistrationID = 4, ReturnURL = "https://SentURL" });

            Assert.True(result?.Success);
            Assert.Equal("https://ReturnedURL", result.Data?.URL);

            _mockData.Verify(d => d.GetAppRegistration(4), Times.Once());
            _mockMastodonApiWrapper.Verify(a => a.GetOAuthUrl("https://SentURL"), Times.Once());
        }

        #endregion App Registration

        #region Mastodon Users

        [Fact]
        public void FollowUserTest()
        {
            _mockMastodonApiWrapper.Setup(d => d.Follow("1", true))
                .Returns<string, bool>((id, follows) => Task.FromResult(new MastodonRelationship { ID = id, Following = follows }));

            var herdApp = new HerdApp(_mockData.Object, _mockMastodonApiWrapper.Object, _mockLogger.Object);

            var result = herdApp.FollowUser(new FollowMastodonUserCommand { UserID = "1", FollowUser = true });

            Assert.True(result?.Success);
            _mockMastodonApiWrapper.Verify(a => a.Follow("1", true), Times.Once());
        }

        [Fact]
        public void UnFollowUserTest()
        {
            _mockMastodonApiWrapper.Setup(d => d.Follow("1", true))
                .Returns<string, bool>((id, follows) => Task.FromResult(new MastodonRelationship { ID = id, Following = follows }));

            var herdApp = new HerdApp(_mockData.Object, _mockMastodonApiWrapper.Object, _mockLogger.Object);

            var result = herdApp.FollowUser(new FollowMastodonUserCommand { UserID = "1", FollowUser = false });

            Assert.True(result?.Success);
            _mockMastodonApiWrapper.Verify(a => a.Follow("1", false), Times.Once());
        }

        [Fact]
        public void SearchUserByNameTest()
        {
            var mockApiWrapperBuilder = new MockMastodonApiWrapperBuilder
            {
                ActiveUserID = "2",
                AllowGetUsersByNameMethod = true
            };
            mockApiWrapperBuilder.SetupUsers(1, 2, 11);
            var mockMastodonApiWrapper = mockApiWrapperBuilder.BuildMockMastodonApiWrapper();
            var herdApp = new HerdApp(_mockData.Object, mockMastodonApiWrapper.Object, _mockLogger.Object);

            var result = herdApp.SearchUsers(new SearchMastodonUsersCommand { Name = "1" });

            Assert.True(result?.Success);
            Assert.Equal(2, result.Data.Users.Count);
            Assert.Equal("1", result.Data.Users[0].MastodonUserId);
            Assert.Equal("11", result.Data.Users[1].MastodonUserId);

            mockMastodonApiWrapper.Verify(a => a.GetUsersByName("1", It.IsAny<MastodonUserContextOptions>(), It.IsAny<PagingOptions>()), Times.Once());
        }

        [Fact]
        public void SearchUserByMastodonUserIdTest()
        {
            var mockApiWrapperBuilder = new MockMastodonApiWrapperBuilder
            {
                ActiveUserID = "2",
                AllowGetMastodonAccountMethod = true
            };
            mockApiWrapperBuilder.SetupUsers(1, 2, 11);
            var mockMastodonApiWrapper = mockApiWrapperBuilder.BuildMockMastodonApiWrapper();
            var herdApp = new HerdApp(_mockData.Object, mockMastodonApiWrapper.Object, _mockLogger.Object);

            var result = herdApp.SearchUsers(new SearchMastodonUsersCommand { UserID = "11" });

            Assert.True(result?.Success);
            Assert.Single(result.Data.Users);
            Assert.Equal("11", result.Data.Users[0].MastodonUserId);

            mockMastodonApiWrapper.Verify(a => a.GetMastodonAccount(It.IsAny<string>(), It.IsAny<MastodonUserContextOptions>()), Times.Once());
        }

        [Fact]
        public void SearchUserIncludingFollowingAndFollowersTest()
        {
            var mockApiWrapperBuilder = new MockMastodonApiWrapperBuilder
            {
                ActiveUserID = "2",
                AllowAddContextToMastodonUsersMethod = true,
                AllowGetMastodonAccountMethod = true,
            };

            mockApiWrapperBuilder.SetupUsers(1, 2, 11);
            mockApiWrapperBuilder.SetupFollowRelationship(1, 2);
            mockApiWrapperBuilder.SetupFollowRelationship(1, 11);
            mockApiWrapperBuilder.SetupFollowRelationship(2, 11);
            mockApiWrapperBuilder.SetupFollowRelationship(11, 2);

            var mockMastodonApiWrapper = mockApiWrapperBuilder.BuildMockMastodonApiWrapper();
            var herdApp = new HerdApp(_mockData.Object, mockMastodonApiWrapper.Object, _mockLogger.Object);

            var result = herdApp.SearchUsers(new SearchMastodonUsersCommand
            {
                UserID = "11",
                IncludeFollowers = true,
                IncludeFollowing = true
            });

            Assert.True(result?.Success);
            Assert.Single(result.Data.Users);
            var user = result.Data.Users[0];
            Assert.Equal("11", user.MastodonUserId);

            Assert.Single(user.Following);
            Assert.Equal("2", user.Following[0].MastodonUserId);

            Assert.Equal(2, user.Followers.Count);
            Assert.Equal("1", user.Followers[0].MastodonUserId);
            Assert.Equal("2", user.Followers[1].MastodonUserId);

            mockMastodonApiWrapper.Verify(a => a.AddContextToMastodonUsers(It.IsAny<IEnumerable<MastodonUser>>(), It.IsAny<MastodonUserContextOptions>()), Times.Once());
            mockMastodonApiWrapper.Verify(a => a.GetMastodonAccount(It.IsAny<string>(), It.IsAny<MastodonUserContextOptions>()), Times.Once());
        }

        [Fact]
        public void SearchUserIncludingFollowingAndFollowedByActiveUserTest()
        {
            var mockApiWrapperBuilder = new MockMastodonApiWrapperBuilder
            {
                ActiveUserID = "2",
                AllowAddContextToMastodonUsersMethod = true,
                AllowGetFollowingMethod = true,
                AllowGetFollowersMethod = true
            };

            mockApiWrapperBuilder.SetupUsers(1, 2, 11);
            mockApiWrapperBuilder.SetupFollowRelationship(1, 2);
            mockApiWrapperBuilder.SetupFollowRelationship(1, 11);
            mockApiWrapperBuilder.SetupFollowRelationship(2, 11);
            mockApiWrapperBuilder.SetupFollowRelationship(11, 2);

            var mockMastodonApiWrapper = mockApiWrapperBuilder.BuildMockMastodonApiWrapper();
            var herdApp = new HerdApp(_mockData.Object, mockMastodonApiWrapper.Object, _mockLogger.Object);

            var result = herdApp.SearchUsers(new SearchMastodonUsersCommand
            {
                FollowsUserID = "11",
                FollowedByUserID = "11",
                IncludeFollowedByActiveUser = true,
                IncludeFollowsActiveUser = true
            });

            Assert.True(result?.Success);
            Assert.Single(result.Data.Users);
            Assert.Equal(1, result.Data.Users.Count);
            Assert.Equal("2", result.Data.Users[0].MastodonUserId);

            mockMastodonApiWrapper.Verify(a => a.AddContextToMastodonUsers(It.IsAny<IEnumerable<MastodonUser>>(), It.IsAny<MastodonUserContextOptions>()), Times.Once());
            mockMastodonApiWrapper.Verify(a => a.GetFollowing(It.IsAny<string>(), It.IsAny<MastodonUserContextOptions>(), It.IsAny<PagingOptions>()), Times.Once());
            mockMastodonApiWrapper.Verify(a => a.GetFollowers(It.IsAny<string>(), It.IsAny<MastodonUserContextOptions>(), It.IsAny<PagingOptions>()), Times.Once());
        }

        #endregion Mastodon Users

        #region Mastodon Posts

        [Fact]
        public void RepostPostTest()
        {
            _mockMastodonApiWrapper.Setup(d => d.Repost("1", true))
                .Returns<string, bool>((id, repost) => Task.FromResult(new MastodonPost { Id = id, IsReblogged = repost }));

            var herdApp = new HerdApp(_mockData.Object, _mockMastodonApiWrapper.Object, _mockLogger.Object);

            var result = herdApp.RepostPost(new RepostMastodonPostCommand { PostID = "1", Repost = true });

            Assert.True(result?.Success);
            _mockMastodonApiWrapper.Verify(a => a.Repost("1", true), Times.Once());
        }

        [Fact]
        public void UnRepostPostTest()
        {
            _mockMastodonApiWrapper.Setup(d => d.Repost("1", false))
                .Returns<string, bool>((id, repost) => Task.FromResult(new MastodonPost { Id = id, IsReblogged = repost }));

            var herdApp = new HerdApp(_mockData.Object, _mockMastodonApiWrapper.Object, _mockLogger.Object);

            var result = herdApp.RepostPost(new RepostMastodonPostCommand { PostID = "1", Repost = false });

            Assert.True(result?.Success);
            _mockMastodonApiWrapper.Verify(a => a.Repost("1", false), Times.Once());
        }

        [Fact]
        public void LikePostTest()
        {
            _mockMastodonApiWrapper.Setup(d => d.Like("1", true))
                .Returns<string, bool>((id, like) => Task.FromResult(new MastodonPost { Id = id, IsFavourited = like }));

            var herdApp = new HerdApp(_mockData.Object, _mockMastodonApiWrapper.Object, _mockLogger.Object);

            var result = herdApp.LikePost(new LikeMastodonPostCommand { PostID = "1", Like = true });

            Assert.True(result?.Success);
            _mockMastodonApiWrapper.Verify(a => a.Like("1", true), Times.Once());
        }

        [Fact]
        public void UnLikePostTest()
        {
            _mockMastodonApiWrapper.Setup(d => d.Like("1", false))
                .Returns<string, bool>((id, like) => Task.FromResult(new MastodonPost { Id = id, IsFavourited = like }));

            var herdApp = new HerdApp(_mockData.Object, _mockMastodonApiWrapper.Object, _mockLogger.Object);

            var result = herdApp.LikePost(new LikeMastodonPostCommand { PostID = "1", Like = false });

            Assert.True(result?.Success);
            _mockMastodonApiWrapper.Verify(a => a.Like("1", false), Times.Once());
        }

        [Fact]
        public void SearchPostsByIdTest()
        {
            var mockApiWrapperBuilder = new MockMastodonApiWrapperBuilder
            {
                AllowAddContextToMastodonPostMethod = true,
                AllowAddContextToMastodonPostsMethod = true,
                AllowGetPostMethod = true,
                ActiveUserID = "2"
            };

            mockApiWrapperBuilder.SetupUsers(1, 2);
            mockApiWrapperBuilder.CreatePost(1, 1);
            mockApiWrapperBuilder.CreatePost(1, 2);

            var mockMastodonApiWrapper = mockApiWrapperBuilder.BuildMockMastodonApiWrapper();

            var herdApp = new HerdApp(_mockData.Object, mockMastodonApiWrapper.Object, _mockLogger.Object);

            var result = herdApp.SearchPosts(new SearchMastodonPostsCommand
            {
                PostID = "1"
            });

            Assert.True(result?.Success);
            Assert.Equal(1, result.Data.Posts.Count);
            Assert.Equal("1", result.Data.Posts[0].Id);

            mockMastodonApiWrapper.Verify(a => a.GetPost(It.IsAny<string>(), It.IsAny<MastodonPostContextOptions>()), Times.Once());
        }

        [Fact]
        public void SearchPostsByAuthorUserIdTest()
        {
            var mockApiWrapperBuilder = new MockMastodonApiWrapperBuilder
            {
                AllowAddContextToMastodonPostMethod = true,
                AllowAddContextToMastodonPostsMethod = true,
                AllowGetPostsByAuthorUserIdMethod = true,
                ActiveUserID = "2"
            };

            mockApiWrapperBuilder.SetupUsers(1, 2);
            mockApiWrapperBuilder.CreatePost(1, 1);
            mockApiWrapperBuilder.CreatePost(1, 2);
            mockApiWrapperBuilder.CreatePost(2, 3);

            var mockMastodonApiWrapper = mockApiWrapperBuilder.BuildMockMastodonApiWrapper();

            var herdApp = new HerdApp(_mockData.Object, mockMastodonApiWrapper.Object, _mockLogger.Object);

            var result = herdApp.SearchPosts(new SearchMastodonPostsCommand
            {
                ByAuthorMastodonUserID = "1"
            });

            Assert.True(result?.Success);
            Assert.Equal(2, result.Data.Posts.Count);
            Assert.Equal("1", result.Data.Posts[0].Id);
            Assert.Equal("2", result.Data.Posts[1].Id);

            mockMastodonApiWrapper.Verify(a => a.GetPostsByAuthorUserID(It.IsAny<string>(), It.IsAny<MastodonPostContextOptions>(), It.IsAny<PagingOptions>()), Times.Once());
        }

        [Fact]
        public void SearchPostsByHashTagTest()
        {
            var mockApiWrapperBuilder = new MockMastodonApiWrapperBuilder
            {
                AllowAddContextToMastodonPostMethod = true,
                AllowAddContextToMastodonPostsMethod = true,
                AllowGetPostsByHashTagMethod = true,
                ActiveUserID = "2"
            };

            mockApiWrapperBuilder.SetupUsers(1, 2);
            mockApiWrapperBuilder.CreatePost(1, 1);
            mockApiWrapperBuilder.CreatePost(1, 2);
            mockApiWrapperBuilder.CreatePost(2, 3);

            var mockMastodonApiWrapper = mockApiWrapperBuilder.BuildMockMastodonApiWrapper();

            var herdApp = new HerdApp(_mockData.Object, mockMastodonApiWrapper.Object, _mockLogger.Object);

            var result = herdApp.SearchPosts(new SearchMastodonPostsCommand
            {
                HavingHashTag = "#post_2"
            });

            Assert.True(result?.Success);
            Assert.Equal(1, result.Data.Posts.Count);
            Assert.Equal("2", result.Data.Posts[0].Id);

            mockMastodonApiWrapper.Verify(a => a.GetPostsByHashTag(It.IsAny<string>(), It.IsAny<MastodonPostContextOptions>(), It.IsAny<PagingOptions>()), Times.Once());
        }

        [Fact]
        public void SearchPostsOnActiveUserTimelineTest()
        {
            var mockApiWrapperBuilder = new MockMastodonApiWrapperBuilder
            {
                AllowAddContextToMastodonPostMethod = true,
                AllowAddContextToMastodonPostsMethod = true,
                AllowGetPostsOnActiveUserTimelineMethod = true,
                ActiveUserID = "2"
            };

            mockApiWrapperBuilder.SetupUsers(1, 2, 3, 4);
            mockApiWrapperBuilder.SetupFollowRelationship(2, 3);
            mockApiWrapperBuilder.SetupFollowRelationship(2, 4);

            mockApiWrapperBuilder.CreatePost(1, 1);
            mockApiWrapperBuilder.CreatePost(1, 2);
            mockApiWrapperBuilder.CreatePost(2, 3);
            mockApiWrapperBuilder.CreatePost(3, 4);
            mockApiWrapperBuilder.CreatePost(3, 5);
            mockApiWrapperBuilder.CreatePost(4, 6);

            var mockMastodonApiWrapper = mockApiWrapperBuilder.BuildMockMastodonApiWrapper();

            var herdApp = new HerdApp(_mockData.Object, mockMastodonApiWrapper.Object, _mockLogger.Object);

            var result = herdApp.SearchPosts(new SearchMastodonPostsCommand
            {
                OnlyOnlyOnActiveUserTimeline = true
            });

            Assert.True(result?.Success);
            Assert.Equal(3, result.Data.Posts.Count);
            Assert.Equal("4", result.Data.Posts[0].Id);
            Assert.Equal("5", result.Data.Posts[1].Id);
            Assert.Equal("6", result.Data.Posts[2].Id);

            mockMastodonApiWrapper.Verify(a => a.GetPostsOnActiveUserTimeline(It.IsAny<MastodonPostContextOptions>(), It.IsAny<PagingOptions>()), Times.Once());
        }

        [Fact]
        public void CreateNewPostTest()
        {
            _mockMastodonApiWrapper
                .Setup(d => d.CreateNewPost("Hello World!", MastodonPostVisibility.Public, null, new string [0], false, null))
                .Returns(Task.FromResult(new MastodonPost()));

            // Create the HerdApp using the mock objects
            var herdApp = new HerdApp(_mockData.Object, _mockMastodonApiWrapper.Object, _mockLogger.Object);

            // Run the HerdApp command (should execute the mock)
            var result = herdApp.CreateNewPost(new CreateNewMastodonPostCommand { Message = "Hello World!" });

            // Verify the result, do we need to check any more than this?
            Assert.True(result?.Success);
            _mockMastodonApiWrapper.Verify(a =>
                a.CreateNewPost("Hello World!", MastodonPostVisibility.Public, null, new string [0], false, null),
                Times.Once());
        }

        #endregion Mastodon Posts
    }
}
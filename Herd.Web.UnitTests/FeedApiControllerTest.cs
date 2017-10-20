using Xunit;
using Moq;
using Herd.Business;
using Herd.Business.Models.Commands;
using Herd.Business.Models;
using Herd.Web.Controllers.HerdApi;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc;
using Herd.Business.Models.CommandResultData;
using System.Collections.Generic;
using Herd.Business.Models.Entities;

namespace Herd.Web.UnitTests
{
    public class FeedApiControllerTest
    {


        [Fact]
        public void NewItemsTest()
        {

            // Mock the app
            var mockApp = new Mock<IHerdApp>();

            // setup the mock
            mockApp.Setup(d => d.GetRecentFeedItems(new GetRecentPostsCommand())).Returns(new CommandResult<GetRecentPostsCommandResultData>
            {
                Data = new GetRecentPostsCommandResultData
                {

                    RecentPosts = new List<MastodonPost>
                {
                    new MastodonPost
                    {
                        Content = "Hello, World!"
                    }
                }

                }
            });

            FeedApiController controller = new FeedApiController();

            // Run the mock
            IActionResult result = controller.NewItems();

            Assert.True(result != null);

        }


        [Fact]
        public void GetStatusTest()
        {


            var mockApp = new Mock<IHerdApp>();

            // setup the mock
            mockApp.Setup(d => d.GetStatus(new GetPostCommand())).Returns(new CommandResult<GetPostCommandResultData> {
                Data = new GetPostCommandResultData
                {
                    MastodonPost = new MastodonPost
                    {
                        Content = "Hello, world!"
                    }
                }
            });

            FeedApiController controller = new FeedApiController();

            // Run the mock
            IActionResult result = controller.GetStatus("1", false, false);

            // Not sure how to test controller beyond this, will researh
            Assert.True(result != null);

        }

        public void NewPostTest() {

            var mockApp = new Mock<IHerdApp>();

            mockApp.Setup(d => d.CreateNewPost(new CreateNewPostCommand())).Returns( new CommandResult());

            FeedApiController controller = new FeedApiController();

            string json = "{ message: '1', visibility: '1', replyStatusID: '0', sensitive: 'false', spoilerTest: 'Starfield' }";
            JObject body = JObject.Parse(json);

            // Run the mock
            IActionResult result = controller.NewPost(body);

            // Not sure how to test controller beyond this, will researh
            Assert.True(result != null);

        }

    }


}

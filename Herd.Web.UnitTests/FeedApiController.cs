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
    class FeedApiControllerTest
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



    }
}

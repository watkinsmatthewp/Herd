using Xunit;
using Moq;
using Herd.Business;
using Herd.Business.Models.Commands;
using Herd.Business.Models;
using Herd.Web.Controllers.HerdApi;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Herd.Web.UnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void MastodonUsersApiControllerFollowTest()
        {

            // Mock the app
            var mockApp = new Mock<IHerdApp>();

            // Effectivly a void method
            mockApp.Setup(d => d.FollowUser(new FollowUserCommand { UserID = "1", FollowUser = true })).Returns(new CommandResult());

            // Make sure no errors occur during call
            MastodonUsersApiController controller = new MastodonUsersApiController();

            string json = "{ mastodonUserID: '1', followUser: 'true' }";
            JObject body = JObject.Parse(json);

            IActionResult result = controller.Follow(body);

            Assert.True(result != null);
        }
    }



}
using Herd.Business.Models.Commands;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Herd.Web.Controllers.HerdApi
{
    [Route("api/feed")]
    public class FeedApiController : BaseApiController
    {
        [HttpGet("new_items")]
        public IActionResult NewItems() => ApiJson(App.GetRecentFeedItems(new HerdAppGetRecentFeedItemsCommand()));

        [HttpGet("get_status")]
        public IActionResult GetStatus(int statusId) => ApiJson(App.GetStatus(new HerdAppGetStatusCommand
        {
            StatusId = statusId
        }));

        [HttpPost("new_post")]
        public IActionResult NewPost([FromBody] JObject body) => ApiJson(App.CreateNewPost(new HerdAppCreateNewPostCommand
        {
            Message = body["message"].Value<string>(),
            Visibility = (Mastonet.Visibility)body["visibility"].Value<int>(), //?? Mastonet.Visibility.Unlisted,
            ReplyStatusId = body["replyStatusId"].Value<int?>(),
            //MediaIds = body["mediaIds"].Value<IEnumerable<int>>(),
            Sensitive = body["sensitive"].Value<bool>(),
            SpoilerText = body["spoilerText"].Value<string>(),
        }));
    }
}
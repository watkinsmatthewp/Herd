using Herd.Business.Models.Commands;
using Herd.Business.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Herd.Web.Controllers.HerdApi
{
    [Route("api/feed")]
    public class FeedApiController : BaseApiController
    {
        [HttpGet("new_items")]
        public IActionResult NewItems() => ApiJson(App.GetRecentFeedItems(new GetRecentPostsCommand()));


        [HttpGet("users_items")]
        public IActionResult UserItems(MastodonUser account) => ApiJson(App.GetRecentUserFeedItems(new GetRecentPostsCommand(), account));

        [HttpGet("get_status")]
        public IActionResult GetStatus(int statusId, bool includeAncestors, bool includeDescendants) => ApiJson(App.GetStatus(new GetPostCommand
        {
            PostID = statusId,
            IncludeAncestors = includeAncestors,
            IncludeDescendants = includeDescendants,
        }));

        [HttpPost("new_post")]
        public IActionResult NewPost([FromBody] JObject body) => ApiJson(App.CreateNewPost(new CreateNewPostCommand
        {
            Message = body["message"].Value<string>(),
            Visibility = (MastodonPostVisibility)body["visibility"].Value<int>(),
            ReplyStatusId = body["replyStatusId"].Value<int?>(),
            //MediaIds = body["mediaIds"].Value<IEnumerable<int>>(),
            Sensitive = body["sensitive"].Value<bool>(),
            SpoilerText = body["spoilerText"].Value<string>(),
        }));
    }
}
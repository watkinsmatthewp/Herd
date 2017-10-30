using Herd.Business.Models.Commands;
using Herd.Business.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Herd.Web.Controllers.HerdApi
{
    [Route("api/mastodon-posts")]
    public class MastodonPostsApiController : BaseApiController
    {
        [HttpGet("search")]
        public IActionResult Search
        (
            bool onlyOnActiveUserTimeline = false,
            string authorMastodonUserID = null,
            string postID = null,
            string hashtag = null,
            bool includeAncestors = false,
            bool includeDescendants = false,
            int max = 30, 
            string maxID = null,
            string sinceID = null 
        )
        {
            return ApiJson(App.SearchPosts(new SearchMastodonPostsCommand
            {
                OnlyOnlyOnActiveUserTimeline = onlyOnActiveUserTimeline,
                ByAuthorMastodonUserID = authorMastodonUserID,
                PostID = postID,
                HavingHashTag = hashtag,
                IncludeAncestors = includeAncestors,
                IncludeDescendants = includeDescendants,
                MaxCount = max,
                MaxID = maxID,
                SinceID = sinceID
            }));
        }

        [HttpPost("new")]
        public IActionResult NewPost([FromBody] JObject body) => ApiJson(App.CreateNewPost(new CreateNewMastodonPostCommand
        {
            Message = body["message"].Value<string>(),
            Visibility = (MastodonPostVisibility)body["visibility"].Value<int>(),
            ReplyStatusId = body["replyStatusId"].Value<string>(),
            // MediaIds = body["mediaIds"].Value<IEnumerable<long>>(),
            Sensitive = body["sensitive"].Value<bool>(),
            SpoilerText = body["spoilerText"].Value<string>(),
        }));

        [HttpPost("repost")]
        public IActionResult RepostPost([FromBody] JObject body)
        {
            return ApiJson(App.RepostPost(new RepostMastodonPostCommand
            {
                PostID = body["statusID"].Value<string>(),
                Repost = body["repost"].Value<bool>()
            }));
        }

        [HttpPost("like")]
        public IActionResult Like([FromBody] JObject body)
        {
            return ApiJson(App.LikePost(new LikeMastodonPostCommand
            {
                PostID = body["statusID"].Value<string>(),
                Like = body["like"].Value<bool>()
            }));
        }
    }
}
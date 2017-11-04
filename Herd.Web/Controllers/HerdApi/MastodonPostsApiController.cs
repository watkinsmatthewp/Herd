using Herd.Business.Models.Commands;
using Herd.Business.Models.Entities;
using Herd.Web.CustomAttributes;
using Herd.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.IO;

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
            int max = 30
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
                MaxCount = max
            }));
        }

        [HttpPost("new")]
        public IActionResult NewPost(NewMastodonPostInputModel post) => ApiJson(App.CreateNewPost(new CreateNewMastodonPostCommand
        {
            Message = post.Message,
            Visibility = post.Visibility,
            ReplyStatusId = post.ReplyStatusId,
            Sensitive = post.Sensitive,
            SpoilerText = post.SpoilerText,
            Attachment = post.Attachment?.OpenReadStream()
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
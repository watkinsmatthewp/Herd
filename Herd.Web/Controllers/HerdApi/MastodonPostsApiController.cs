using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Herd.Business.Models.Commands;
using Newtonsoft.Json.Linq;
using Herd.Business.Models.Entities;

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
        public IActionResult NewPost([FromBody] JObject body) => ApiJson(App.CreateNewPost(new CreateNewMastodonPostCommand
        {
            Message = body["message"].Value<string>(),
            Visibility = (MastodonPostVisibility)body["visibility"].Value<int>(),
            ReplyStatusId = body["replyStatusId"].Value<string>(),
            // MediaIds = body["mediaIds"].Value<IEnumerable<long>>(),
            Sensitive = body["sensitive"].Value<bool>(),
            SpoilerText = body["spoilerText"].Value<string>(),
        }));
    }
}
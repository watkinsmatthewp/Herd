using Herd.Business.Models;
using Herd.Business.Models.Commands;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Herd.Web.Controllers.HerdApi
{
    [Route("api/mastodon-users")]
    public class MastodonUsersApiController : BaseApiController
    {
        [HttpGet("search")]
        public IActionResult Search
        (
            string mastodonUserID = null,
            string name = null,
            string followsMastodonUserID = null,
            string followedByMastodonUserID = null,
            bool includeFollowers = false,
            bool includeFollowing = false,
            bool includeFollowsActiveUser = false,
            bool includeFollowedByActiveUser = false,
            string maxID = null,
            string sinceID = null,
            int? max = 30
        )
        {
            return ApiJson(App.SearchUsers(new SearchMastodonUsersCommand
            {
                UserID = mastodonUserID,
                Name = name,
                FollowedByUserID = followedByMastodonUserID,
                FollowsUserID = followsMastodonUserID,
                IncludeFollowers = includeFollowers,
                IncludeFollowing = includeFollowing,
                IncludeFollowedByActiveUser = includeFollowedByActiveUser,
                IncludeFollowsActiveUser = includeFollowsActiveUser,
                PagingOptions = new PagingOptions
                {
                    Limit = max,
                    MaxID = maxID,
                    SinceID = sinceID
                }
            }));
        }

        [HttpPost("follow")]
        public IActionResult Follow([FromBody] JObject body)
        {
            return ApiJson(App.FollowUser(new FollowMastodonUserCommand
            {
                UserID = body["mastodonUserID"].Value<string>(),
                FollowUser = body["followUser"].Value<bool>(),
            }));
        }

        [HttpPost("update")]
        public IActionResult UpdateMastodonUser([FromBody] JObject body)
        {
            return ApiJson(App.UpdateUserMastodonProfile(new UpdateUserMastodonProfileCommand
            {
                DisplayName = body["display_name"].Value<string>(),
                Bio = body["bio"].Value<string>(),
                Avatar = body["avatar"].Value<string>(),
                Header = body["header"].Value<string>(),
            }));
        }
    }
}
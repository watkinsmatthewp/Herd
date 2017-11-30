using Herd.Business.Models;
using Herd.Business.Models.Commands;
using Herd.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;

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
        public IActionResult UpdateMastodonUser(UpdateMastodonProfileInputModel update)
        {
            return ApiJson(App.UpdateUserMastodonProfile(new UpdateUserMastodonProfileCommand
            {
                DisplayName = update.DisplayName,
                Bio = update.Bio,
                AvatarImageStream = update.AvatarImage?.OpenReadStream(),
                HeaderImageStream = update.HeaderImage?.OpenReadStream()
            }));
        }

        [HttpGet("notifications")]
        public IActionResult GetNotifications
        (
            bool includeAncestors = false,
            bool includeDescendants = false,
            int max = 30,
            string maxID = null,
            string sinceID = null
        )
        {
            return ApiJson(App.GetNotifications(new GetMastodonNotificationsCommand
            {
                IncludeAncestors = includeAncestors,
                IncludeDescendants = includeDescendants,
                PagingOptions = new PagingOptions
                {
                    Limit = max,
                    MaxID = maxID,
                    SinceID = sinceID
                }
            }));
        }
    }
}
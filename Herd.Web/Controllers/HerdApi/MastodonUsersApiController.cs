using Herd.Business;
using Herd.Business.Models.Commands;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Herd.Web.Controllers.HerdApi
{
    [Route("api/mastodon-users")]
    public class MastodonUsersApiController : BaseApiController
    {
        [HttpGet("search")]
        public IActionResult Search
        (
            long? mastodonUserID = null,
            string name = null,
            long? followsMastodonUserID = null,
            long? followedByMastodonUserID = null,
            bool includeFollowers = false,
            bool includeFollowing = false,
            bool includeFollowsActiveUser = false,
            bool includeFollowedByActiveUser = false,
            int max = 30
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
                MaxCount = max
            }));
        }
    }
}
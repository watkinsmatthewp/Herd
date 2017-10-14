using Herd.Business;
using Herd.Business.Models.Commands;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Herd.Web.Controllers.HerdApi
{
    [Route("api/mastodon-users")]
    public class MastodonUsersController : BaseApiController
    {
        [HttpGet("search")]
        public IActionResult Search(int? userID = null, string name = null, int? followsUserID = null, int? followedByUserID = null, int max = 30)
        {
            return ApiJson(App.SearchUsers(new SearchMastodonUsersCommand
            {
                UserID = userID,
                Name = name,
                FollowedByUserID = followedByUserID,
                FollowsUserID = followsUserID,
                MaxCount = max
            }));
        }
    }
}
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
        public async Task<IActionResult> Search(int? userID = null, string name = null, int? followsUserID = null, int? followedByUserID = null, int max = 30)
        {
            // TODO: Temporary lookup so that we can inject ourselves into dummy data
            if (HerdApp.DUMMY_USERS.Count == 4)
            {
                HerdApp.DUMMY_USERS.Add(await _mastodonApiWrapper.Value.GetActiveUserMastodonAccount());
            }

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
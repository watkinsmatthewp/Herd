using Herd.Business;
using Herd.Business.Models.CommandResultData;
using Herd.Business.Models.Commands;
using Microsoft.AspNetCore.Mvc;
using System;
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
                var mastodonUser = await _mastodonApiWrapper.Value.GetUserAccount();
                var names = mastodonUser.DisplayName.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                HerdApp.DUMMY_USERS.Add(new SearchMastodonUsersCommandResultData.UserSearchResult
                {
                    MastodonDisplayName = mastodonUser.DisplayName,
                    MastodonHeaderImageURL = mastodonUser.HeaderUrl,
                    MastodonProfileImageURL = mastodonUser.AvatarUrl,
                    MastodonShortBio = mastodonUser.Note,
                    MastodonUserID = mastodonUser.Id,
                    MastodonUserName = mastodonUser.UserName
                });
            }
            // END TODO

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
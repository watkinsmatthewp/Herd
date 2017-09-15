using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Herd.Business;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Herd.Web.Controllers
{
    [Route("api/[controller]")]
    public class AuthApiController : BaseController
    {
        [HttpGet("[action]")]
        public async Task<IActionResult> GetMastodonOAuthURL()
        {
            return new ObjectResult(new { url = await MastodonApiWrapper.GetOAuthUrl("urn:ietf:wg:oauth:2.0:oob") });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> LoginWithOAuthToken([FromBody] JObject data)
        {
            var oAuthToken = data["oAuthToken"].Value<string>();

            try
            {
                await MastodonApiWrapper.LoginWithOAuthToken(oAuthToken);

                // Get Mastodon User 
                var activeUser = await MastodonApiWrapper.GetUserAccount();

                // Update ActiveUser with Mastodon user?
                ActiveUser.UserName = activeUser.UserName;
                ActiveUser.ID = activeUser.Id;
                ActiveUser.ApiAccessToken = oAuthToken;
                HerdApp.Instance.Data.UpdateUser(ActiveUser);
                return Ok(new { successful = true });
            } catch (Exception)
            {
                return BadRequest("Invalid API Token");
            }
        }

        [HttpGet("[action]")]
        public IActionResult GetUsername()
        {
            var activeUser = ActiveUser;
            return new ObjectResult(new { username = ActiveUser.UserName, id = ActiveUser.ID });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Herd.Business;
using Newtonsoft.Json.Linq;
using Mastonet.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Herd.Web.Controllers
{
    [Route("api/[controller]")]
    public class AuthApiController : BaseController
    {
        /**
         * Attempts to log the user in. If the user can not 
         */
        [HttpPost("[action]")]
        public async Task<IActionResult> LoginToApp([FromBody] JObject body)
        {
            var username = body["username"].Value<string>();
            var instance = body["instance"].Value<string>();

            // Sets the instances
            MastodonApiWrapper.HostInstance = instance;
            MastodonApiWrapper.SetAuthClientInstance(instance);

            var loggedIn = await MastodonApiWrapper.LoginToApp(username, instance);
            if (loggedIn)
            {
                var activeUser = await MastodonApiWrapper.GetUserAccount();

                var mastodonUser = await MastodonApiWrapper.GetUserAccount();
                // Update ActiveUser with Mastodon user?
                UpdateActiveUser(mastodonUser, MastodonApiWrapper.UserApiToken);

                // TODO return some auth access token?
                return new ObjectResult(new { loginSuccessful = true });
            } else
            {
                return new ObjectResult(new
                {
                    loginSuccessful = false,
                    url = await MastodonApiWrapper.GetOAuthUrl("urn:ietf:wg:oauth:2.0:oob", instance),
                });
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> LoginWithOAuthToken([FromBody] JObject body)
        {
            var oAuthToken = body["oAuthToken"].Value<string>();
            var instance = body["instance"].Value<string>();

            try
            {
                await MastodonApiWrapper.LoginWithOAuthToken(instance, oAuthToken);
                // Get Mastodon User 
                var mastodonUser = await MastodonApiWrapper.GetUserAccount();
                // Update ActiveUser with Mastodon user?
                UpdateActiveUser(mastodonUser, oAuthToken);
                return Ok(new { successful = true });
            } catch (Exception)
            {
                return BadRequest("Invalid API Token");
            }
        }

        public void UpdateActiveUser(Account mastodonAccount, string oAuthToken = null)
        {
            
            ActiveUser.UserName = mastodonAccount.UserName;
            ActiveUser.ID = mastodonAccount.Id;
            ActiveUser.ApiAccessToken = oAuthToken;
            HerdApp.Instance.Data.UpdateUser(ActiveUser);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Herd.Business;
using Newtonsoft.Json.Linq;
using Mastonet.Entities;
using Herd.Data.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Herd.Web.Controllers
{
    [Route("api/[controller]")]
    public class AuthApiController : BaseController
    {
        [HttpGet]
        public IActionResult Logout()
        {
            // TODO Logout User
            return Ok();
        }

        /**
         * Attempts to log the user in. If the user can not 
         */
        [HttpPost("[action]")]
        public async Task<IActionResult> LoginToApp([FromBody] JObject body)
        {
            var username = body["username"].Value<string>();
            var instance = body["instance"].Value<string>();
            MastodonApiWrapper.HostInstance = instance;
            await MastodonApiWrapper.SetAuthClientInstance(instance);

            var loggedIn = await MastodonApiWrapper.LoginToApp(username, instance);
            if (loggedIn)
            {
                var activeUser = await MastodonApiWrapper.GetUserAccount();
                var mastodonUser = await MastodonApiWrapper.GetUserAccount();
                UpdateActiveUser(mastodonUser, instance, MastodonApiWrapper.UserApiToken); // Update ActiveUser with Mastodon user?
                return new ObjectResult(new { loginSuccessful = true }); // TODO return some auth access token for our website?
            } else
            {
                return new ObjectResult(new
                {
                    loginSuccessful = false,
                    url = await MastodonApiWrapper.GetOAuthUrl("urn:ietf:wg:oauth:2.0:oob", instance),
                });
            }
        }

        /**
         * Login with the User supplied OAuth token 
         */
        [HttpPost("[action]")]
        public async Task<IActionResult> LoginWithOAuthToken([FromBody] JObject body)
        {
            var oAuthToken = body["oAuthToken"].Value<string>();
            var instance = body["instance"].Value<string>();

            MastodonApiWrapper.UserApiToken = oAuthToken;
            MastodonApiWrapper.HostInstance = instance;
            try
            {
                await MastodonApiWrapper.LoginWithOAuthToken(instance, oAuthToken);
                var mastodonUser = await MastodonApiWrapper.GetUserAccount();
                UpdateActiveUser(mastodonUser, instance, oAuthToken); // Update ActiveUser with Mastodon user?
                return Ok(new { successful = true }); // TODO return some auth access token for our website?
            } catch (Exception)
            {
                return BadRequest("Invalid Authentication");
            }
        }

        #region Helper Methods
        public void UpdateActiveUser(Account mastodonAccount, string instance, string oAuthToken)
        {
            HerdApp.Instance.Data.UpdateUser(new HerdUserDataModel
            {
                ID = mastodonAccount.Id,
                MastodonInstanceHost = instance,
                UserName = mastodonAccount.UserName,
                ApiAccessToken = oAuthToken,
            }, $"{instance}@{mastodonAccount.UserName}");
        }
        #endregion
    }
}

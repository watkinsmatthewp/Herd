using Herd.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Herd.Web.Controllers.HerdApi
{
    [Route("api/oauth")]
    public class OAuthApiController : BaseApiController
    {
        [HttpGet("url")]
        public IActionResult GetMastodonInstanceOAuthURL(string username, string instance)
        {
            // Build the user from the login details and store it in the cookie
            _activeUser = new Lazy<HerdUserDataModel>(new HerdUserDataModel
            {
                UserName = username,
                MastodonInstanceHost = instance
            });

            SetActiveUserCookie(ActiveUser);

            // Build the redirect URL to send to Mastodon's API
            var redirectURL = RequestedURL.Contains("localhost")
                ? "urn:ietf:wg:oauth:2.0:oob"
                : RequestedURL.Replace("url", "return");

            // Return the Mastodon OAuth URL
            return new ObjectResult(new
            {
                url = MastodonApiWrapper.GetOAuthUrl(redirectURL)
            });
        }

        [HttpPost("tokens")]
        public IActionResult SetMastodonOAuthTokens([FromBody] JObject body)
        {
            ActiveUser.ApiAccessToken = body["token"].Value<string>();
            SetActiveUserCookie(ActiveUser);
            return Ok();
        }
    }
}

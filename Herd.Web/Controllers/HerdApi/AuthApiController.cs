using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Herd.Business;
using Newtonsoft.Json.Linq;
using Mastonet.Entities;
using Herd.Data.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Herd.Web.Controllers
{
    [Route("api/auth")]
    public class AuthApiController : BaseApiController
    {
        protected override bool RequiresAuthentication(ActionExecutingContext context) => false;

        [HttpPost("[action]")]
        public async Task<IActionResult> oauth_url([FromBody] JObject body)
        {
            // Build the user from the login details and store it in the cookie
            _activeUser = new Lazy<HerdUserDataModel>(new HerdUserDataModel
            {
                UserName = body["username"].Value<string>(),
                MastodonInstanceHost = body["instance"].Value<string>()
            });

            SetActiveUserCookie(ActiveUser);

            // Build the redirect URL to send to Mastodon's API
            var redirectURL = RequestedURL.Contains("localhost")
                ? "urn:ietf:wg:oauth:2.0:oob"
                : RequestedURL.Replace(nameof(oauth_url), nameof(oauth_return));
            
            // Return the Mastodon OAuth URL
            return new ObjectResult(new
            {
                url = MastodonApiWrapper.GetOAuthUrl(redirectURL)
            });
        }

        [HttpGet("[action]")]
        public IActionResult oauth_return(string code)
        {
            ActiveUser.ApiAccessToken = code;
            SetActiveUserCookie(ActiveUser);
            return Ok();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            ClearActiveUser();
            return Ok();
        }
    }
}

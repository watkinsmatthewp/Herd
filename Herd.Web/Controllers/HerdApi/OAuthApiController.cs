using Herd.Data.Models;
using Herd.Web.CustomAttributes;
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
        private const string NON_REDIRECT_URL = "urn:ietf:wg:oauth:2.0:oob";

        [HttpPost("login"), AuthenticationNotRequired]
        public IActionResult Login([FromBody] JObject body)
        {
            _activeUser = new Lazy<HerdUserDataModel>(new HerdUserDataModel
            {
                UserName = body["username"].Value<string>(),
                MastodonInstanceHost = body["instance"].Value<string>()
            });

            // Validate passed in Auth
            bool validAuth = true;


            if (validAuth)
            {
                SetActiveUserCookie(ActiveUser);
            } else
            {
                // send error message
            }
            

            return new ObjectResult(new
            {

            });
        }

        [HttpGet("logout"), AuthenticationNotRequired]
        public IActionResult Logout()
        {
            this.ClearActiveUser();
            return Ok();
        }

        [HttpGet("url")]
        public IActionResult GetMastodonInstanceOAuthURL(string instance)
        {
            return new ObjectResult(new
            {
                url = Business.MastodonApiWrapper.GetOAuthUrl(instance, NON_REDIRECT_URL)
            });
        }

        [HttpPost("set_tokens")]
        public IActionResult SetMastodonOAuthTokens([FromBody] JObject body)
        {
            ActiveUser.MastodonInstanceHost = body["instance"].Value<string>();
            ActiveUser.ApiAccessToken = body["token"].Value<string>();
            SetActiveUserCookie(ActiveUser);
            return Ok();
        }
    }
}

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

        [HttpGet("url")]
        public IActionResult GetMastodonInstanceOAuthURL(string instance)
        {
            return ApiJson(new
            {
                url = Business.MastodonApiWrapper.GetOAuthUrl(instance, NON_REDIRECT_URL)
            });
        }

        [HttpPost("set_tokens")]
        public IActionResult SetMastodonOAuthTokens([FromBody] JObject body)
        {
            ActiveUser.MastodonConnection = new HerdUserMastodonConnectionDetails
            {
                ApiAccessToken = body["token"].Value<string>(),
                Instance = body["instance"].Value<string>()
            };
            SetActiveUserCookie(ActiveUser);
            return Ok();
        }
    }
}

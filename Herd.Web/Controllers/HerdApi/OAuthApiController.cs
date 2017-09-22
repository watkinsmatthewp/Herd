using Herd.Business;
using Herd.Business.Models.Commands;
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

        [HttpGet("registration_id")]
        public IActionResult GetAppRegistrationID(string instance)
        {
            var result = HerdApp.Instance.GetOrCreateRegistration(new HerdAppGetOrCreateRegistrationCommand
            {
                ApiWrapper = MastodonApiWrapper,
                Instance = instance
            });

            // Visible to user. Only expose necessary fields
            if (result.Data != null)
            {
                result.Data.Registration = ClearUnnecessaryOrSensitiveData(result.Data.Registration);
            }

            return ApiJson(result);
        }

        [HttpGet("url")]
        public IActionResult GetMastodonInstanceOAuthURL(long registrationID, string returnURL = null)
        {
            return ApiJson(HerdApp.Instance.GetOAuthURL(new HerdAppGetOAuthURLCommand
            {
                ApiWrapper = MastodonApiWrapper,
                AppRegistrationID = registrationID,
                ReturnURL = returnURL
            }));
        }

        [HttpPost("set_tokens")]
        public IActionResult SetMastodonOAuthTokens([FromBody] JObject body)
        {
            ActiveUser.MastodonConnection = new HerdUserMastodonConnectionDetails
            {
                ApiAccessToken = body["token"].Value<string>(),
                AppRegistrationID = body["app_registration_id"].Value<long>()
            };
            HerdApp.Instance.Data.UpdateUser(ActiveUser);
            return Ok();
        }

        #region Private helpers

        private HerdAppRegistrationDataModel ClearUnnecessaryOrSensitiveData(HerdAppRegistrationDataModel registration) => new HerdAppRegistrationDataModel
        {
            ID = registration.ID
        };

        #endregion
    }
}

using Herd.Business;
using Herd.Business.Models.Commands;
using Herd.Core;
using Herd.Data.Models;
using Herd.Web.CustomAttributes;
using Mastonet;
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
        [HttpGet("registration_id")]
        public IActionResult GetAppRegistrationID(string instance)
        {
            _mastodonApiWrapper = new Lazy<IMastodonApiWrapper>(new MastodonApiWrapper(instance));

            var result = App.GetOrCreateRegistration(new HerdAppGetOrCreateRegistrationCommand
            {
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
        public IActionResult GetMastodonInstanceOAuthURL(long registrationID)
        {
            _appRegistration = new Lazy<HerdAppRegistrationDataModel>(DataProvider.GetAppRegistration(registrationID));
            _mastodonApiWrapper = new Lazy<IMastodonApiWrapper>(new MastodonApiWrapper(AppRegistration));

            return ApiJson(App.GetOAuthURL(new HerdAppGetOAuthURLCommand
            {
                AppRegistrationID = registrationID
            }));
        }

        [HttpPost("set_tokens")]
        public IActionResult SetMastodonOAuthTokens([FromBody] JObject body)
        {
            var oneTimeUserApiAccessToken = body["token"].Value<string>();
            var appRegistrationID = body["app_registration_id"].Value<long>();

            // TODO: All the code below is here only temporarily. It doesn't belong here. 
            // We need to clean it up, wrap it in a command, and move it to the business layer

            // Get the app registration for the ID provided
            var appRegistration = DataProvider.GetAppRegistration(appRegistrationID);

            // Connect with the one-time use code to get the permanent code
            var authClient = new AuthenticationClient(appRegistration.ToMastodonAppRegistration());
            var auth = authClient.ConnectWithCode(oneTimeUserApiAccessToken).Synchronously();
            ActiveUser.MastodonConnection = auth.ToHerdConnectionDetails(appRegistrationID);
            DataProvider.UpdateUser(ActiveUser);

            // END TODO

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

using Herd.Business;
using Herd.Business.Models.Commands;
using Herd.Data.Models;
using Herd.Web.Code;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;

namespace Herd.Web.Controllers.HerdApi
{
    [Route("api/oauth")]
    public class OAuthApiController : BaseApiController
    {
        [HttpGet("registration_id")]
        public IActionResult GetAppRegistrationID(string instance)
        {
            _mastodonApiWrapper = new Lazy<IMastodonApiWrapper>(new MastodonApiWrapper(instance));

            var result = App.GetOrCreateRegistration(new GetOrCreateRegistrationCommand
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
            _appRegistration = new Lazy<Registration>(HerdWebApp.Instance.DataProvider.GetAppRegistration(registrationID));
            _mastodonApiWrapper = new Lazy<IMastodonApiWrapper>(new MastodonApiWrapper(AppRegistration));

            return ApiJson(App.GetOAuthURL(new GetOAuthURLCommand
            {
                AppRegistrationID = registrationID
            }));
        }

        [HttpPost("set_tokens")]
        public IActionResult SetMastodonOAuthTokens([FromBody] JObject body)
        {
            _appRegistration = new Lazy<Registration>(HerdWebApp.Instance.DataProvider.GetAppRegistration(body["app_registration_id"].Value<long>()));
            _mastodonApiWrapper = new Lazy<IMastodonApiWrapper>(new MastodonApiWrapper(AppRegistration));

            return ApiJson(App.UpdateUserMastodonConnection(new UpdateUserMastodonConnectionCommand
            {
                AppRegistrationID = body["app_registration_id"].Value<long>(),
                Token = body["token"].Value<string>(),
                UserID = ActiveUser.ID
            }));
        }

        #region Private helpers

        private Registration ClearUnnecessaryOrSensitiveData(Registration registration) => new Registration
        {
            ID = registration.ID
        };

        #endregion Private helpers
    }
}
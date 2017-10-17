using Herd.Business;
using Herd.Business.ApiWrappers;
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
        public IActionResult GetMastodonInstanceOAuthURL(int registrationID)
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
            _appRegistration = new Lazy<Registration>(HerdWebApp.Instance.DataProvider.GetAppRegistration(body["app_registration_id"].Value<int>()));
            _mastodonApiWrapper = new Lazy<IMastodonApiWrapper>(new MastodonApiWrapper(AppRegistration));

            var result = App.UpdateUserMastodonConnection(new UpdateUserMastodonConnectionCommand
            {
                AppRegistrationID = body["app_registration_id"].Value<string>(),
                Token = body["token"].Value<string>(),
                UserID = ActiveUser.ID
            });

            // Visible to user. Only expose necessary fields
            if (result.Data != null)
            {
                result.Data.User = ClearUnnecessaryOrSensitiveData(result.Data.User);
            }

            return ApiJson(result);
        }

        #region Private helpers

        private Registration ClearUnnecessaryOrSensitiveData(Registration registration) => new Registration
        {
            ID = registration.ID
        };

        private UserAccount ClearUnnecessaryOrSensitiveData(UserAccount userAccount) => new UserAccount
        {
            ID = userAccount.ID,
            MastodonConnection = new UserMastodonConnectionDetails
            {
                MastodonUserID = userAccount.MastodonConnection.MastodonUserID
            }
        };

        #endregion Private helpers
    }
}
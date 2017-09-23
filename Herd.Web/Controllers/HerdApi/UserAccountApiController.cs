using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Herd.Web.CustomAttributes;
using Herd.Data.Models;
using Newtonsoft.Json.Linq;
using Herd.Business;
using Herd.Business.Models.Commands;

namespace Herd.Web.Controllers.HerdApi
{
    [Route("api/account")]
    public class UserAccountApiController : BaseApiController
    {
        [HttpPost("register"), AuthenticationNotRequired]
        public IActionResult Register([FromBody] JObject body)
        {
            var result = HerdApp.Instance.CreateUser(new HerdAppCreateUserCommand
            {
                Email = body["email"].Value<string>(),
                FirstName = body["firstName"].Value<string>(),
                LastName = body["lastName"].Value<string>(),
                PasswordPlainText = body["password"].Value<string>()
            });

            // Visible to user. Only expose necessary fields
            if (result.Data != null)
            {
                result.Data.User = ClearUnnecessaryOrSensitiveData(result.Data.User);
            }

            return ApiJson(result);
        }

        [HttpPost("login"), AuthenticationNotRequired]
        public IActionResult Login([FromBody] JObject body)
        {
            var result = HerdApp.Instance.LoginUser(new HerdAppLoginUserCommand
            {
                Email = body["email"].Value<string>(),
                PasswordPlainText = body["password"].Value<string>()
            });

            if (result.Success)
            {
                SetActiveUserCookie(result.Data.User);
            }

            // Visible to user. Only expose necessary fields
            if (result.Data != null)
            {
                result.Data.User = ClearUnnecessaryOrSensitiveData(result.Data.User);
            }

            return ApiJson(result);
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            ClearActiveUserCookie();
            return Ok();
        }

        #region Private helpers

        private HerdUserAccountDataModel ClearUnnecessaryOrSensitiveData(HerdUserAccountDataModel userAccount) => new HerdUserAccountDataModel
        {
            ID = userAccount.ID,
            ProfileID = userAccount.ProfileID,
            MastodonConnection = userAccount.MastodonConnection
        };

        #endregion
    }
}
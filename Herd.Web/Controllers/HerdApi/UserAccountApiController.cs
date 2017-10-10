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
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Herd.Web.Code;
using Herd_Web;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Herd.Web.Controllers.HerdApi
{
    [Route("api/account")]
    public class UserAccountApiController : BaseApiController
    {
        [HttpPost("register"), AuthenticationNotRequired]
        public IActionResult Register([FromBody] JObject body)
        {
            var result = App.CreateUser(new HerdAppCreateUserCommand
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
        public async Task<IActionResult> Login([FromBody] JObject body)
        {
            var result = App.LoginUser(new HerdAppLoginUserCommand
            {
                Email = body["email"].Value<string>(),
                PasswordPlainText = body["password"].Value<string>()
            });

            if (result.Success)
            {
                await this.SetActiveUserInSession(result.Data.User);
            }

            // Visible to user. Only expose necessary fields
            if (result.Data != null)
            {
                result.Data.User = ClearUnnecessaryOrSensitiveData(result.Data.User);
            }

            return ApiJson(result);
        }

        [HttpGet("logout"), AuthenticationNotRequired]
        public async Task<IActionResult> Logout()
        {
            await this.ClearActiveUserFromSession();
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
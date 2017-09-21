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
            return new ObjectResult(HerdApp.Instance.CreateUser(new HerdAppCreateUserCommand
            {
                Email = body["email"].Value<string>(),
                FirstName = body["firstname"].Value<string>(),
                LastName = body["lastname"].Value<string>(),
                PasswordPlainText = body["password"].Value<string>()
            }));
        }

        [HttpPost("login"), AuthenticationNotRequired]
        public IActionResult Login([FromBody] JObject body)
        {
            var result = HerdApp.Instance.LoginUser(new HerdAppLoginUserCommand
            {
                Email = body["email"].Value<string>(),
                PasswordPlainText = body["instance"].Value<string>()
            });

            if (result.Success)
            {
                SetActiveUserCookie(result.Data.User);
            }

            return new ObjectResult(result);
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            ClearActiveUserCookie();
            return Ok();
        }
    }
}
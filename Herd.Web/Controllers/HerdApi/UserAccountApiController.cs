using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Herd.Web.CustomAttributes;
using Herd.Data.Models;
using Newtonsoft.Json.Linq;
using Herd.Business;

namespace Herd.Web.Controllers.HerdApi
{
    [Route("api/account")]
    public class UserAccountApiController : BaseApiController
    {
        [HttpPost("register"), AuthenticationNotRequired]
        public IActionResult Register([FromBody] JObject body)
        {
            var user = HerdApp.Instance.Data.CreateUser(new HerdUserDataModel
            {
                Email = body["email"].Value<string>(),
                FirstName = body["firstname"].Value<string>(),
                LastName = body["lastname"].Value<string>(),
                Password = body["password"].Value<string>(),
            });
            return Ok();
        }

        [HttpPost("login"), AuthenticationNotRequired]
        public IActionResult Login([FromBody] JObject body)
        {
            _activeUser = new Lazy<HerdUserDataModel>(new HerdUserDataModel
            {
                Email = body["username"].Value<string>(),
                MastodonInstanceHost = body["instance"].Value<string>()
            });

            // Validate passed in Auth
            bool validAuth = true;


            if (validAuth)
            {
                SetActiveUserCookie(ActiveUser);
            }
            else
            {
                // send error message
            }


            return new ObjectResult(new
            {

            });
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            this.ClearActiveUser();
            return Ok();
        }
    }
}
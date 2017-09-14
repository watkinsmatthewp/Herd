using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Herd.Web.Controllers
{
    [Route("api/[controller]")]
    public class AuthApiController : BaseController
    {
        #region Random Number Region
        [HttpGet("[action]")]
        public IActionResult GetRandomNumber()
        {
            Random r = new Random();
            return new ObjectResult(new NumberObject { Numero = r.Next(5, 500) });
        }
        #endregion

        [HttpGet("[action]/{instance}")]
        public async Task<IActionResult> ConnectToMastodon(string instance)
        {

            return Ok();
        }

        [HttpGet("[action]/{oAuthToken}")]
        public IActionResult SubmitOAuthToken(string oAuthToken)
        {

            return Ok();
        }
    }

    public class MastodonAuthentication
    {
        public string Mastodon_Instance { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string OAuthUrl { get; set; }
        public string AccessToken { get; set; }
    }

    public class NumberObject
    {
        public int Numero { get; set; }
    }
}

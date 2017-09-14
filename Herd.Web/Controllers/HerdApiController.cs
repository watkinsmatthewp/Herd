using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mastonet;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Herd.Controllers
{

    // api/Mastodon
    [Route("api/[controller]")]
    public class HerdApiController : Controller
    {
        private Mastonet.AuthenticationClient client = null;
        private Mastonet.Entities.AppRegistration appRegistration = null;
        private Mastonet.Entities.Account account = null;

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
            this.client = new AuthenticationClient(instance);
            this.appRegistration = await this.client.CreateApp("Herd", Scope.Read | Scope.Write | Scope.Follow);

            var request = Request;
            var host = $"https://www.google.com/";
            var url = this.client.OAuthUrl(host);
            return new ObjectResult(new MastodonAuthentication { OAuthUrl = url });
        }

        [HttpGet("[action]/{oAuthToken}")]
        public IActionResult SubmitOAuthToken(string oAuthToken)
        {
            // Couldn't get this part to work because the client wasn't persisting across api calls.

            //var accessToken = "not this";
            //this.client = new AuthenticationClient("octodon.social");
            //var auth = await this.client.ConnectWithCode(accessToken);
            //// From auth we need to save AccessToken into a Database, if AccessToken exists we can use that to login with later times.

            //var client = new MastodonClient(this.appRegistration, auth);
            //this.account = await client.GetCurrentUser();

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

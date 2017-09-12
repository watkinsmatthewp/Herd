using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Herd.Business;
using Herd.Core;

namespace Herd.Web.Controllers
{
    public class MastodonAuthController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> OAuthRedirect()
        {
            // For local testing, use the weird redirect URL (see https://github.com/doorkeeper-gem/doorkeeper/wiki/Authorization-Code-Flow)
            // This tells the OAuth endpoint to make the user copy the code :(
            // TODO: No real OAuth for loalhost?
            var returnURL = RequestedURL.Contains("localhost") ? "urn:ietf:wg:oauth:2.0:oob" : RequestedURL.Replace("OAuthRedirect", "OAuthReturn");
            var oAuthURL = await MastodonApiWrapper.GetOAuthUrl(returnURL);
            return Redirect(oAuthURL);
        }

        // TODO: Same controller as above, but synchronous instead of async
        //public IActionResult OAuth()
        //{
        //    var oAuthURL = MastodonApiWrapper.GetOAuthUrl(RequestedURL).Synchronously();
        //    return Redirect(oAuthURL);
        //}

        public IActionResult OAuthReturn(string code)
        {
            // code is the access code sent back from mastodon in the user's URL (may have different name-- need to investigate)
            // Update the user in the database and send them back to the Index page
            ActiveUser.ApiAccessToken = code;
            HerdApp.Instance.Data.UpdateUser(ActiveUser);
            SuccessMessage = "Successfully authenticated";
            return RedirectToAction("Index");
        }
    }
}

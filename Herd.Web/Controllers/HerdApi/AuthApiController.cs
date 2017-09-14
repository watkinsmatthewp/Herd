using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Herd.Business;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Herd.Web.Controllers
{
    [Route("api/[controller]")]
    public class AuthApiController : BaseController
    {
        [HttpGet("[action]")]
        public async Task<IActionResult> GetMastodonOAuthURL()
        {
            return new ObjectResult(new { url = await MastodonApiWrapper.GetOAuthUrl("urn:ietf:wg:oauth:2.0:oob") });
        }

        [HttpPost("[action]/{oAuthToken}")]
        public IActionResult SaveOAuthToken(string oAuthToken)
        {
            ActiveUser.ApiAccessToken = oAuthToken;
            HerdApp.Instance.Data.UpdateUser(ActiveUser);
            return new ObjectResult(new { success = true });
        }
    }
}

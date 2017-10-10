using Microsoft.AspNetCore.Mvc;

namespace Herd.Web.Controllers.HerdApi
{
    public class BaseApiController : BaseController
    {
        protected override IActionResult NotAuthorized()
        {
            Response.StatusCode = 401;
            return new ObjectResult(new
            {
                message = $"The session cookie value is not supplied"
            });
        }
    }
}
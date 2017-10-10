using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Herd.Web.Controllers
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

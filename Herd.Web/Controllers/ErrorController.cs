using Herd.Web.CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Herd.Web.Controllers
{
    [Route("/error")]
    public class ErrorController : Controller
    {
        [AuthenticationNotRequired]
        public IActionResult Index(Guid? id = null)
        {
            return View(id);
        }

        [Route("/json"), AuthenticationNotRequired]
        public IActionResult ErrorJSON(Guid? id = null)
        {
            return new ObjectResult(new
            {
                ID = id,
                Message = "An error occurred"
            });
        }
    }
}
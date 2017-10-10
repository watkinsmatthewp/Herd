using Herd.Web.Controllers;
using Herd.Web.CustomAttributes;
using Microsoft.AspNetCore.Mvc;

namespace Herd_Web.Controllers
{
    public class HomeController : BaseController
    {
        [AuthenticationNotRequired]
        public IActionResult Index()
        {
            return View();
        }
    }
}
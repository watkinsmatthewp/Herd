using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Herd.Web.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Herd_Web.Controllers
{
    public class HomeController : BaseController
    {
        protected override bool RequiresAuthentication(ActionExecutingContext context) => false;

        public IActionResult Index()
        {
            return View();
        }
    }
}

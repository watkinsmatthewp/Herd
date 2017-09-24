using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Herd.Web.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Herd.Web.CustomAttributes;

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

using Herd.Business;
using Herd.Data.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Herd.Core;
using Microsoft.AspNetCore.Mvc.Controllers;
using Herd.Web.CustomAttributes;
using Herd.Business.Models.Commands;
using Herd.Data.Providers;

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
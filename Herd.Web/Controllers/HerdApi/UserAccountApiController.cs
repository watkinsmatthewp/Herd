using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Herd.Web.Controllers.HerdApi
{
    public class UserAccountApiController : BaseApiController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
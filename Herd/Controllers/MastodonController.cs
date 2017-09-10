using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Herd.Controllers
{
    // api/Mastodon
    [Route("api/[controller]")]
    public class MastodonController : Controller
    {
        [HttpGet("[action]")]
        public IActionResult GetRandomNumber()
        {
            Random r = new Random();
            return new ObjectResult(new NumberObject { Numero = r.Next(5, 500) });
        }
    }

    public class NumberObject
    {
        public int Numero { get; set; }
    }
}

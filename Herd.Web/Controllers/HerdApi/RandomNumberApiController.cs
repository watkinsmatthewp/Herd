using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Herd.Web.Controllers
{
    [Route("api/[controller]")]
    public class RandomNumberApiController : BaseApiController
    {
        private static readonly Random _rand = new Random(Guid.NewGuid().GetHashCode());

        [HttpGet("[action]")]
        public IActionResult GetRandomNumber()
        {
            return new ObjectResult(new { Numero = _rand.Next(5, 500) });
        }
    }
}

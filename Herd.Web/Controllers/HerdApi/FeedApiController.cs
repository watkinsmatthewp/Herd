using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Herd.Web.Controllers
{
    [Route("api/feed")]
    public class FeedApiController : BaseApiController
    {
        [HttpGet("new_items")]
        public IActionResult NewItems()
        {
            return new ObjectResult(new Post[]
            {
                new Post{ Text = "Hello, Morgan.", Author = "Jacob" },
                new Post{ Text = "Another post", Author = "Thomas" },
                new Post{ Text = "Yet another post", Author = "Matthew" }
            });
        }
    }

    public class Post
    {
        public string Text { get; set; }
        public string Author { get; set; }
    }
}

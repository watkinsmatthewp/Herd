﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Herd.Business;
using Herd.Business.Models.Commands;
using Newtonsoft.Json.Linq;

namespace Herd.Web.Controllers
{
    [Route("api/feed")]
    public class FeedApiController : BaseApiController
    {
        [HttpGet("new_items")]
        public IActionResult NewItems() => ApiJson(App.GetRecentFeedItems(new HerdAppGetRecentFeedItemsCommand()));

        [HttpPost("new_post")]
        public IActionResult NewPost([FromBody] JObject body) => ApiJson(App.CreateNewPost(new HerdAppCreateNewPostCommand
        {
            Message = body["message"].Value<string>()
        }));
    }
}
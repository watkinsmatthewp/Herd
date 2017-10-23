using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.Commands
{
    public class RepostMastodonPostCommand
    {
        public string PostID { get; set; }
        public bool Repost { get; set; }
    }
}

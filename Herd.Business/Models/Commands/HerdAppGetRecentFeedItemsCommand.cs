using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.Commands
{
    public class HerdAppGetRecentFeedItemsCommand : HerdAppCommand
    {
        public int MaxCount { get; set; } = 30;
        public string BeforeFeedID { get; set; }
    }
}

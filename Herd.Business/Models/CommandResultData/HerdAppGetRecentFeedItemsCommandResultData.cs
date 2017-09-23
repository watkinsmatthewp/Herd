using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.CommandResultData
{
    public class HerdAppGetRecentFeedItemsCommandResultData : HerdAppCommandResultData
    {
        public class RecentFeedItem
        {
            public string Text { get; set; }
        }

        public List<RecentFeedItem> RecentFeedItems { get; set; }
    }
}

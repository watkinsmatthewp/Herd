using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.CommandResultData
{
    public class HerdAppGetRecentFeedItemsCommandResultData : HerdAppCommandResultData
    {
        public List<Mastonet.Entities.Status> RecentFeedItems { get; set; }
    }
}

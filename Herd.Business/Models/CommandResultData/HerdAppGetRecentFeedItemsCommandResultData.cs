using Herd.Business.Models.MastodonWrappers;
using System.Collections.Generic;

namespace Herd.Business.Models.CommandResultData
{
    public class HerdAppGetRecentFeedItemsCommandResultData : HerdAppCommandResultData
    {
        public List<Status> RecentFeedItems { get; set; }
    }
}
using Herd.Business.Models.MastodonWrappers;
using System.Collections.Generic;

namespace Herd.Business.Models.CommandResultData
{
    public class GetRecentFeedItemsCommandResultData : CommandResultData
    {
        // TODO: Replace with non-MastoNet object ASAP
        public List<Status> RecentFeedItems { get; set; }
    }
}
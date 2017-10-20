using Herd.Business.Models.Entities;
using System.Collections.Generic;

namespace Herd.Business.Models.CommandResultData
{
    public class GetRecentPostsCommandResultData : CommandResultDataObject
    {
        public IList<MastodonPost> RecentPosts { get; set; }
    }
}
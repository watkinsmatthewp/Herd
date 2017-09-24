using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.CommandResultData
{
    public class HerdAppGetRecentFeedItemsCommandResultData : HerdAppCommandResultData
    {
        public class RecentFeedItem
        {
            public string AuthorUserName { get; set;}
            public string AuthorDisplayname { get; set; }
            public string AuthorAvatarURL { get; set; }
            public string Text { get; set; }
        }

        public List<Mastonet.Entities.Status> RecentFeedItems { get; set; }
    }
}

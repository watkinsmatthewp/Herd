using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.Commands
{
    public class SearchMastodonPostsCommand : Command
    {
        public int MaxCount { get; set; } = 30;

        public bool OnlyOnlyOnActiveUserTimeline { get; set; }
        public string ByAuthorMastodonUserID { get; set; }
        public string PostID { get; set; }
        public string HavingHashTag { get; set; }

        public bool IncludeAncestors { get; set; }
        public bool IncludeDescendants { get; set; }

        public bool IsGlobalSearch => !OnlyOnlyOnActiveUserTimeline
            && string.IsNullOrWhiteSpace(ByAuthorMastodonUserID)
            && string.IsNullOrWhiteSpace(PostID)
            && string.IsNullOrWhiteSpace(HavingHashTag);
    }
}

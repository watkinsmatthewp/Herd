namespace Herd.Business.Models.Commands
{
    public class SearchMastodonPostsCommand : Command
    {
        public int MaxCount { get; set; } = 30;
        public string MaxID { get; set; }
        public string SinceID { get; set; }

        public bool OnlyOnlyOnActiveUserTimeline { get; set; }
        public string ByAuthorMastodonUserID { get; set; }
        public string PostID { get; set; }
        public string HavingHashTag { get; set; }

        public bool IncludeAncestors { get; set; }
        public bool IncludeDescendants { get; set; }

        public bool IsGlobalSearch => !OnlyOnlyOnActiveUserTimeline
            && string.IsNullOrWhiteSpace(ByAuthorMastodonUserID)
            && string.IsNullOrWhiteSpace(PostID)
            && string.IsNullOrWhiteSpace(HavingHashTag)
            && string.IsNullOrWhiteSpace(MaxID)
            && string.IsNullOrWhiteSpace(SinceID);
    }
}
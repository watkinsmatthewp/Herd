namespace Herd.Business.Models.Commands
{
    public class SearchMastodonPostsCommand : Command, IPagedCommand
    {
        public PagingOptions PagingOptions { get; set; }

        public bool OnlyOnActiveUserTimeline { get; set; }
        public bool OnlyOnPublicTimeline { get; set; }
        public string ByAuthorMastodonUserID { get; set; }
        public string PostID { get; set; }
        public string HavingHashTag { get; set; }

        public bool IncludeAncestors { get; set; }
        public bool IncludeDescendants { get; set; }

        public bool IsGlobalSearch => !OnlyOnActiveUserTimeline && !OnlyOnPublicTimeline
            && string.IsNullOrWhiteSpace(ByAuthorMastodonUserID)
            && string.IsNullOrWhiteSpace(PostID)
            && string.IsNullOrWhiteSpace(HavingHashTag)
            && !this.HasPagingOptions();
    }
}
namespace Herd.Business.Models.Commands
{
    public class GetRecentPostsCommand : Command
    {
        public bool IncludeInReplyToPost { get; set; }
        public bool IncludeReplyPosts { get; set; }
        public string SincePostID { get; set; }
        public string MaxPostID { get; set; }
        public int MaxCount { get; set; } = 30;
    }
}
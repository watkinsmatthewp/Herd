namespace Herd.Business.Models.Commands
{
    public class GetRecentPostsCommand : Command
    {
        public bool IncludeInReplyToPost { get; set; }
        public bool IncludeReplyPosts { get; set; }
        public long? SincePostID { get; set; }
        public long? MaxPostID { get; set; }
        public int MaxCount { get; set; } = 30;
    }
}
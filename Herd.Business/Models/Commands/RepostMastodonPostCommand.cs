namespace Herd.Business.Models.Commands
{
    public class RepostMastodonPostCommand
    {
        public string PostID { get; set; }
        public bool Repost { get; set; }
    }
}
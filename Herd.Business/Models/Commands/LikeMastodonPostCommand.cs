namespace Herd.Business.Models.Commands
{
    public class LikeMastodonPostCommand
    {
        public string PostID { get; set; }
        public bool Like { get; set; }
    }
}
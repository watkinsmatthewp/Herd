namespace Herd.Business.Models.Commands
{
    public class GetRecentFeedItemsCommand : Command
    {
        public int MaxCount { get; set; } = 30;
        public string BeforeFeedID { get; set; }
    }
}
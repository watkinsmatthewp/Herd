namespace Herd.Business.Models.Commands
{
    public class GetPostCommand : Command
    {
        public string PostID { get; set; }
        public bool IncludeAncestors { get; set; } = false;
        public bool IncludeDescendants { get; set; } = false;
    }
}
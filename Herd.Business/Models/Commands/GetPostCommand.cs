namespace Herd.Business.Models.Commands
{
    public class GetPostCommand : Command
    {
        public int PostID { get; set; }
        public bool IncludeAncestors { get; set; } = false;
        public bool IncludeDescendants { get; set; } = false;
    }
}
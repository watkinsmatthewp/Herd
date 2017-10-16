namespace Herd.Business.Models.Commands
{
    public class GetPostCommand : Command
    {
        public long PostID { get; set; }
        public bool IncludeAncestors { get; set; } = false;
        public bool IncludeDescendants { get; set; } = false;
    }
}
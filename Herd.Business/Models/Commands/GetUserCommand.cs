namespace Herd.Business.Models.Commands
{
    public class GetUserCommand : Command
    {
        public long UserID { get; set; }
    }
}
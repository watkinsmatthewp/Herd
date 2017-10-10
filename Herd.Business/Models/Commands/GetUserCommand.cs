namespace Herd.Business.Models.Commands
{
    public class GetUserCommand : HerdAppCommand
    {
        public long UserID { get; set; }
    }
}
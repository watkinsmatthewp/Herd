namespace Herd.Business.Models.Commands
{
    public class GetUserCommand : Command
    {
        public string UserID { get; set; }
    }
}
namespace Herd.Business.Models.Commands
{
    public class GetHerdUserCommand : Command
    {
        public string UserID { get; set; }
    }
}
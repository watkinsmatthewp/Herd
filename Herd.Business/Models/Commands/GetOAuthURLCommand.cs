namespace Herd.Business.Models.Commands
{
    public class GetOAuthURLCommand : Command
    {
        public string AppRegistrationID { get; set; }
        public string ReturnURL { get; set; }
    }
}
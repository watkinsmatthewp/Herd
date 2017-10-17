namespace Herd.Business.Models.Commands
{
    public class GetOAuthURLCommand : Command
    {
        public int AppRegistrationID { get; set; }
        public string ReturnURL { get; set; }
    }
}
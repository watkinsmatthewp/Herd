namespace Herd.Business.Models.Commands
{
    public class GetOAuthURLCommand : Command
    {
        public long AppRegistrationID { get; set; }
        public string ReturnURL { get; set; }
    }
}
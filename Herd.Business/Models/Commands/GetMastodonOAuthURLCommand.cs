namespace Herd.Business.Models.Commands
{
    public class GetMastodonOAuthURLCommand : Command
    {
        public int AppRegistrationID { get; set; }
        public string ReturnURL { get; set; }
    }
}
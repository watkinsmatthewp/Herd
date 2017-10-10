namespace Herd.Business.Models.Commands
{
    public class GetOAuthURLCommand : HerdAppCommand
    {
        public long AppRegistrationID { get; set; }
        public string ReturnURL { get; set; }
    }
}
namespace Herd.Business.Models.Commands
{
    public class UpdateUserMastodonConnectionCommand : Command
    {
        public long UserID { get; set; }
        public long AppRegistrationID { get; set; }
        public string Token { get; set; }
    }
}
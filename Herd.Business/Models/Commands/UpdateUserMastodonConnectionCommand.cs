namespace Herd.Business.Models.Commands
{
    public class UpdateUserMastodonConnectionCommand : Command
    {
        public int UserID { get; set; }
        public int AppRegistrationID { get; set; }
        public string Token { get; set; }
    }
}
namespace Herd.Business.Models.Commands
{
    public class UpdateHerdUserMastodonConnectionCommand : Command
    {
        public int UserID { get; set; }
        public int AppRegistrationID { get; set; }
        public string Token { get; set; }
    }
}
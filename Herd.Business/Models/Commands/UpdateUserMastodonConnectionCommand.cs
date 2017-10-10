namespace Herd.Business.Models.Commands
{
    public class UpdateUserMastodonConnectionCommand : HerdAppCommand
    {
        public long UserID { get; set; }
        public long AppRegistrationID { get; set; }
        public string Token { get; set; }
    }
}
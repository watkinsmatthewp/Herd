namespace Herd.Data.Models
{
    public class HerdUserAccountSecurity
    {
        public long SaltKey { get; set; }
        public string SaltedPassword { get; set; }
    }

    public class HerdUserMastodonConnectionDetails
    {
        public long AppRegistrationID { get; set; }
        public string ApiAccessToken { get; set; }
    }

    public interface IHerdUserAccountDataModel : IHerdDataModel
    {
        string Email { get; set; }
        long ProfileID { get; set; }
        HerdUserAccountSecurity Security { get; set; }
        HerdUserMastodonConnectionDetails MastodonConnection { get; set; }
    }

    public class HerdUserAccountDataModel : HerdDataModel, IHerdUserAccountDataModel
    {
        public string Email { get; set; }
        public long ProfileID { get; set; }
        public HerdUserAccountSecurity Security { get; set; }
        public HerdUserMastodonConnectionDetails MastodonConnection { get; set; }
    }
}
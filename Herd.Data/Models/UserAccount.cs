namespace Herd.Data.Models
{
    public class UserAccountSecurity
    {
        public long SaltKey { get; set; }
        public string SaltedPassword { get; set; }
    }

    public class UserMastodonConnectionDetails
    {
        public long AppRegistrationID { get; set; }
        public string ApiAccessToken { get; set; }
        public string CreatedAt { get; set; }
        public string Scope { get; set; }
        public string TokenType { get; set; }
    }

    public interface IUserAccount : IDataModel
    {
        string Email { get; set; }
        long ProfileID { get; set; }
        UserAccountSecurity Security { get; set; }
        UserMastodonConnectionDetails MastodonConnection { get; set; }
    }

    public class UserAccount : DataModel, IUserAccount
    {
        public string Email { get; set; }
        public long ProfileID { get; set; }
        public UserAccountSecurity Security { get; set; }
        public UserMastodonConnectionDetails MastodonConnection { get; set; }
    }
}
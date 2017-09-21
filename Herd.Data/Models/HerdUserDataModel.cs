namespace Herd.Data.Models
{
    public interface IHerdUserDataModel : IHerdDataModel
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        string Email { get; set; }
        long SaltKey { get; set; }
        string SaltedPassword { get; set; }
        string MastodonInstanceHost { get; set; }
        string ApiAccessToken { get; set; }
    }

    public class HerdUserDataModel : HerdDataModel, IHerdUserDataModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public long SaltKey { get; set; }
        public string SaltedPassword { get; set; }
        public string MastodonInstanceHost { get; set; }
        public string ApiAccessToken { get; set; }
    }
}
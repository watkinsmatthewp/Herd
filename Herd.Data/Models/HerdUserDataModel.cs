namespace Herd.Data.Models
{
    public interface IHerdUserDataModel : IHerdDataModel
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        string Email { get; set; }
        string Password { get; set; }
        string MastodonInstanceHost { get; set; }
        string ApiAccessToken { get; set; }
    }

    public class HerdUserDataModel : HerdDataModel, IHerdUserDataModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string MastodonInstanceHost { get; set; }
        public string ApiAccessToken { get; set; }
    }
}
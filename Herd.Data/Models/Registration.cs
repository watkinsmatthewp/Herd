namespace Herd.Data.Models
{
    public interface IRegistration : IDataModel
    {
        int MastodonAppRegistrationID { get; }
        string ClientId { get; }
        string ClientSecret { get; }
        string Instance { get; }
    }

    public class Registration : DataModel, IRegistration
    {
        public int MastodonAppRegistrationID { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Instance { get; set; }
    }
}
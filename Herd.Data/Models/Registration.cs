namespace Herd.Data.Models
{
    public class Registration : DataModel
    {
        public string MastodonAppRegistrationID { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Instance { get; set; }
    }
}
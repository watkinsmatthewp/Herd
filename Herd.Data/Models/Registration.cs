namespace Herd.Data.Models
{
    public class Registration : DataModel
    {
        public int MastodonAppRegistrationID { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Instance { get; set; }
    }
}
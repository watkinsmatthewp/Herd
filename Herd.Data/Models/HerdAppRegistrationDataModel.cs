namespace Herd.Data.Models
{
    public interface IHerdAppRegistrationDataModel : IHerdDataModel
    {
        int RegistrationID { get; }
        string ClientId { get; }
        string ClientSecret { get; }
        string Instance { get; }
    }

    public class HerdAppRegistrationDataModel : HerdDataModel, IHerdAppRegistrationDataModel
    {
        public int RegistrationID { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Instance { get; set; }
    }
}
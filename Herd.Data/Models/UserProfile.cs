namespace Herd.Data.Models
{
    public interface IUserProfile : IDataModel
    {
        long UserID { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
    }

    public class UserProfile : DataModel, IUserProfile
    {
        public long UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
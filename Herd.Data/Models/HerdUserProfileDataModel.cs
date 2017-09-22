using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Data.Models
{
    public interface IHerdUserProfileDataModel : IHerdDataModel
    {
        long UserID { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
    }

    public class HerdUserProfileDataModel : HerdDataModel, IHerdUserProfileDataModel
    {
        public long UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}

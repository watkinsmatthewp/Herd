using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Data.Models
{
    public interface IHerdAppRegistrationDataModel : IHerdDataModel
    {
        string RedirectUri { get; }
        string ClientId { get; }
        string ClientSecret { get; }
        string Instance { get; }
    }

    public class HerdAppRegistrationDataModel : HerdDataModel, IHerdAppRegistrationDataModel
    {
        public string RedirectUri { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Instance { get; set; }
    }
}

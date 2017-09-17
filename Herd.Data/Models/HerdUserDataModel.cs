using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Data.Models
{
    public interface IHerdUserDataModel : IHerdDataModel
    {
        string UserName { get; }
        string MastodonInstanceHost { get; }
        string ApiAccessToken { get; }
        string Email { get; }
    }

    public class HerdUserDataModel : HerdDataModel, IHerdUserDataModel
    {
        public string UserName { get; set; }
        public string MastodonInstanceHost { get; set; }
        public string ApiAccessToken { get; set; }
        public string Email { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.Commands
{
    public class HerdAppGetOAuthURLCommand : HerdAppCommand
    {
        public IMastodonApiWrapper ApiWrapper { get; set; }
        public long AppRegistrationID { get; set; }
        public string ReturnURL { get; set; }
    }
}

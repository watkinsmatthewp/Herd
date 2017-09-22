using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.Commands
{
    public class HerdAppGetOrCreateRegistrationCommand : HerdAppCommand
    {
        public IMastodonApiWrapper ApiWrapper { get; set; }
        public string Instance { get; set; }
    }
}

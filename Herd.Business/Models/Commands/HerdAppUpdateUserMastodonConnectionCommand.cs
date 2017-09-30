using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.Commands
{
    public class HerdAppUpdateUserMastodonConnectionCommand : HerdAppCommand
    {
        public long UserID { get; set; }
        public long AppRegistrationID { get; set; }
        public string Token { get; set; }
    }
}

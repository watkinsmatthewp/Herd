using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.Commands
{
    public class HerdAppGetOAuthURLCommand : HerdAppCommand
    {
        public long AppRegistrationID { get; set; }
        public string ReturnURL { get; set; }
    }
}

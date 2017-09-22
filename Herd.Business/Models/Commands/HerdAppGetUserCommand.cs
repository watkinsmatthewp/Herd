using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.Commands
{
    public class HerdAppGetUserCommand : HerdAppCommand
    {
        public long UserID { get; set; }
    }
}

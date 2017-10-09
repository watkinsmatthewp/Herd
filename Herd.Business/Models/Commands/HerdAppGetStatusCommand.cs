using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.Commands
{
    public class HerdAppGetStatusCommand : HerdAppCommand
    {
        public int StatusId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.Commands
{
    public class HerdAppGetRegistrationCommand : HerdAppCommand
    {
        public long ID { get; set; }
    }
}

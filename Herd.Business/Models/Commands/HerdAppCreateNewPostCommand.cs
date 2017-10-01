using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.Commands
{
    public class HerdAppCreateNewPostCommand : HerdAppCommand
    {
        public string Message { get; set; }
    }
}

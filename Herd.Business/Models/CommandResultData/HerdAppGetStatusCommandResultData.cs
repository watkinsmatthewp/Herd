using Herd.Business.Models.MastodonWrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.CommandResultData
{
    public class HerdAppGetStatusCommandResultData : HerdAppCommandResultData
    {
        public Status Status { get; set; }
        public StatusContext StatusContext { get; set; } 
    }
}

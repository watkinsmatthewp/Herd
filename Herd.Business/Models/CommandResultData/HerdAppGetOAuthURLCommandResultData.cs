using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.CommandResultData
{
    public class HerdAppGetOAuthURLCommandResultData : HerdAppCommandResultData
    {
        public string URL { get; set; }
    }
}

using Herd.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.CommandResultData
{
    public class HerdAppGetOrCreateRegistrationCommandResultData : HerdAppCommandResultData
    {
        public HerdAppRegistrationDataModel Registration { get; set; }
    }
}

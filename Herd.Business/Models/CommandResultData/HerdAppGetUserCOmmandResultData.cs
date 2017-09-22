using Herd.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.CommandResultData
{
    public class HerdAppGetUserCommandResultData : HerdAppCommandResultData
    {
        public HerdUserAccountDataModel User { get; set; }
    }
}

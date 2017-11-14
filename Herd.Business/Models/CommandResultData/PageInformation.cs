using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.CommandResultData
{
    public class PageInformation
    {
        public string EarlierPageMaxID { get; set; }
        public string NewerPageSinceID { get; set; }
    }
}

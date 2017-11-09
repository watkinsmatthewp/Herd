using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.CommandResultData
{
    public interface IPagedResult
    {
        PageInformation PageInformation { get; set; }
    }
}

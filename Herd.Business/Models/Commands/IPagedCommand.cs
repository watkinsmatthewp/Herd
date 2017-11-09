using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.Commands
{
    public interface IPagedCommand
    {
        PagingOptions PagingOptions { get; set; }
    }
}

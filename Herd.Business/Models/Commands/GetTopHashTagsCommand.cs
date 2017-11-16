using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.Commands
{
    public class GetTopHashTagsCommand : Command
    {
        public int Limit { get; set; } = 30;
    }
}

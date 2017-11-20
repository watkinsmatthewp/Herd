using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.Commands
{
    public class GetMastodonNotificationsCommand
    {
        public PagingOptions PagingOptions { get; set; }
        public bool IncludeAncestors { get; set; }
        public bool IncludeDescendants { get; set; }
    }
}

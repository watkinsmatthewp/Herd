using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.ApiWrappers.MastodonObjectContextOptions
{
    public class MastodonPostContextOptions
    {
        public bool IncludeAncestors { get; internal set; }
        public bool IncludeDescendants { get; internal set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.ApiWrappers.MastodonObjectContextOptions
{
    public class MastodonUserContextOptions
    {
        public bool IncludeFollowers { get; set; }
        public bool IncludeFollowing { get; set; }
        public bool IncludeIsFollowedByActiveUser { get; set; }
        public bool IncludeFollowsActiveUser { get; set; }
    }
}

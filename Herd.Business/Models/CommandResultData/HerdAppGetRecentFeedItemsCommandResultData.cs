﻿using Herd.Business.Models.MastodonWrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.CommandResultData
{
    public class HerdAppGetRecentFeedItemsCommandResultData : HerdAppCommandResultData
    {
        public List<Status> RecentFeedItems { get; set; }
    }
}

using Herd.Business.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.CommandResultData
{
    public class SearchMastodonPostsCommandResultData : CommandResultDataObject
    {
        public List<MastodonPost> Posts { get; set; }
    }
}

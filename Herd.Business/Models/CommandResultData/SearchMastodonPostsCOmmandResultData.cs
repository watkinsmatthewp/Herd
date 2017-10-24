using Herd.Business.Models.Entities;
using System.Collections.Generic;

namespace Herd.Business.Models.CommandResultData
{
    public class SearchMastodonPostsCommandResultData : CommandResultDataObject
    {
        public IList<MastodonPost> Posts { get; set; }
    }
}
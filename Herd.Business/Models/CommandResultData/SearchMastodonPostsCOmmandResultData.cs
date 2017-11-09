using Herd.Business.Models.Entities;
using System.Collections.Generic;

namespace Herd.Business.Models.CommandResultData
{
    public class SearchMastodonPostsCommandResultData : CommandResultDataObject, IPagedResult
    {
        public IList<MastodonPost> Posts { get; set; }
        public PageInformation PageInformation { get; set; }
    }
}
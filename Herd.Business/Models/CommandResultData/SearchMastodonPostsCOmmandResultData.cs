using Herd.Business.Models.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Herd.Business.Models.CommandResultData
{
    public class SearchMastodonPostsCommandResultData : CommandResultDataObject, IPagedResult
    {
        [JsonProperty("Items")]
        public IList<MastodonPost> Posts { get; set; }
        public PageInformation PageInformation { get; set; }
    }
}
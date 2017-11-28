using Herd.Business.Models.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Herd.Business.Models.CommandResultData
{
    public class SearchMastodonUsersCommandResultData : CommandResultDataObject, IPagedResult
    {
        [JsonProperty("Items")]
        public IList<MastodonUser> Users { get; set; }
        public PageInformation PageInformation { get; set; }
    }
}
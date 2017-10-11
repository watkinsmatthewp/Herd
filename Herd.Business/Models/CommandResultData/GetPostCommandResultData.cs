using Herd.Business.Models.Entities;

namespace Herd.Business.Models.CommandResultData
{
    public class GetPostCommandResultData : CommandResultDataObject
    {
        public MastodonPost MastodonPost { get; set; }
    }
}
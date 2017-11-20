using Herd.Business.Models.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.CommandResultData
{

    public class GetMastodonNotificationCommandResultData : CommandResultDataObject, IPagedResult
    {
        [JsonProperty("Items")]
        public IList<MastodonNotification> Notifications { get; set; }
        public PageInformation PageInformation { get; set; }
    }
}

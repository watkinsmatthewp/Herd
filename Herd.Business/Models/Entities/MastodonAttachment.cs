using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.Entities
{
    public class MastodonAttachment
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public string RemoteUrl { get; set; }
        public string PreviewUrl { get; set; }
        public string TextUrl { get; set; }
    }
}

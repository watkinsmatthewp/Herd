using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.Entities
{
    public class MastodonNotification
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public MastodonUser Account { get; set; }
        public MastodonPost Status { get; set; }
    }
}

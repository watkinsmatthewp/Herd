using System;
using System.Collections.Generic;

namespace Herd.Business.Models.Entities
{
    public class MastodonPost
    {
        // Core properties
        public int ID { get; set; }

        public MastodonPostVisibility Visibility { get; set; }
        public MastodonUser Author { get; set; }
        public string SpoilerText { get; set; }
        public string Content { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public int? InReplyToPostID { get; set; }
        public bool? IsSensitive { get; set; }

        // Extra "context" properties
        public MastodonPost InReplyToPost { get; set; }

        public List<MastodonPost> Replies { get; set; }
    }
}
using Mastonet.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.MastodonWrappers
{
    public class Status
    {
        public MastodonWrappers.Account Account { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool? Favourited { get; set; }
        public int FavouritesCount { get; set; }
        public int Id { get; set; }
        public int? InReplyToAccountId { get; set; }
        public int? InReplyToId { get; set; }
        public IEnumerable<Attachment> MediaAttachments { get; set; }
        public IEnumerable<Mention> Mentions { get; set; }
        public Mastonet.Entities.Status Reblog { get; set; }
        public int ReblogCount { get; set; }
        public bool? Reblogged { get; set; }
        public bool? Sensitive { get; set; }
        public string SpoilerText { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
        public string Uri { get; set; }
        public string Url { get; set; }
        public Mastonet.Visibility Visibility { get; set; }
    }
}

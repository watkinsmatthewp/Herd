using Herd.Business.Models.Entities;
using System.Collections.Generic;

namespace Herd.Business.Models.Commands
{
    public class CreateNewPostCommand : Command
    {
        public string Message { get; set; }
        public MastodonPostVisibility Visibility { get; set; }
        public int? ReplyStatusId { get; set; }
        public IEnumerable<int> MediaIds { get; set; }
        public bool Sensitive { get; set; }
        public string SpoilerText { get; set; }
    }
}
using Herd.Business.Models.Entities;
using System.Collections.Generic;

namespace Herd.Business.Models.Commands
{
    public class CreateNewMastodonPostCommand : Command
    {
        public string Message { get; set; }
        public MastodonPostVisibility Visibility { get; set; }
        public string ReplyStatusId { get; set; }
        public IEnumerable<string> MediaIds { get; set; }
        public bool Sensitive { get; set; }
        public string SpoilerText { get; set; }
    }
}
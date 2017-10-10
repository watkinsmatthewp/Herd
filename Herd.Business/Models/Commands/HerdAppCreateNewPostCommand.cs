using System.Collections.Generic;

namespace Herd.Business.Models.Commands
{
    public class HerdAppCreateNewPostCommand : HerdAppCommand
    {
        public string Message { get; set; }
        public Mastonet.Visibility Visibility { get; set; }
        public int? ReplyStatusId { get; set; }
        public IEnumerable<int> MediaIds { get; set; }
        public bool Sensitive { get; set; }
        public string SpoilerText { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.Commands
{
    public class FollowMastodonUserCommand : Command
    {
        public string UserID { get; set; }
        public bool FollowUser { get; set; }
    }
}

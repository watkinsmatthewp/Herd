using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.Commands
{
    public class FollowUserCommand : Command
    {
        public long UserID { get; set; }
        public bool FollowUser { get; set; }
    }
}

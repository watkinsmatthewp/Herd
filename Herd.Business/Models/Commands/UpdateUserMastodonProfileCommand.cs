using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.Commands
{
    public class UpdateUserMastodonProfileCommand : Command
    {
        public string DisplayName { get; set;}
        public string Bio { get; set; }
        public string Avatar { get; set; }
        public string Header { get; set; }
    }
}

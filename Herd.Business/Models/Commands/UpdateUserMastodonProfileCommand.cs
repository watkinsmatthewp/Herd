using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Herd.Business.Models.Commands
{
    public class UpdateUserMastodonProfileCommand : Command
    {
        public string DisplayName { get; set;}
        public string Bio { get; set; }
        public Stream AvatarImageStream { get; set; }
        public Stream HeaderImageStream { get; set; }
    }
}

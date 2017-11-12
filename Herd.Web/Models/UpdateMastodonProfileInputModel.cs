using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Herd.Web.Models
{
    public class UpdateMastodonProfileInputModel
    {
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public string Avatar { get; set; }
        public string Header { get; set; }
    }
}

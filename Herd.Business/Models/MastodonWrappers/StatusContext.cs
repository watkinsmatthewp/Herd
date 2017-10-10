using System.Collections.Generic;

namespace Herd.Business.Models.MastodonWrappers
{
    public class StatusContext
    {
        public List<Status> Ancestors { get; set; }
        public List<Status> Descendants { get; set; }
    }
}
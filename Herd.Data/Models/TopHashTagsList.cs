using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Data.Models
{
    public class TopHashTagsList : DataModel
    {
        public SortedSet<HashTag> HashTags { get; set; } = new SortedSet<HashTag>();
    }
}

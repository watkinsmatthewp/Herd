using Herd.Business.Models.Entities;
using Herd.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.CommandResultData
{
    public class GetTopHashTagsCommandResultData : CommandResultDataObject
    {
        public IList<HashTag> HashTags { get; set; } = new List<HashTag>();
    }
}

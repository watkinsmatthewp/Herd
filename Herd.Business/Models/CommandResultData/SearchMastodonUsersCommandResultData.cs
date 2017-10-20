using Herd.Business.Models.Entities;
using System.Collections.Generic;

namespace Herd.Business.Models.CommandResultData
{
    public class SearchMastodonUsersCommandResultData : CommandResultDataObject
    {
        public IList<MastodonUser> Users { get; set; }
    }
}
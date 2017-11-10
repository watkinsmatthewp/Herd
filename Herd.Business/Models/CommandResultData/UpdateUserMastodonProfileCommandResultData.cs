using Herd.Business.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.CommandResultData
{
    public class UpdateUserMastodonProfileCommandResultData : CommandResultDataObject
    {
        public MastodonUser UpdatedUser { get; set; }
    }
}

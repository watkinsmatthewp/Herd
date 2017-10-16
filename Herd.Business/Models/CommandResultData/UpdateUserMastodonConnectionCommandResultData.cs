using Herd.Data.Models;

namespace Herd.Business.Models.CommandResultData
{
    public class UpdateUserMastodonConnectionCommandResultData : CommandResultDataObject
    {
        public UserAccount User { get; set; }
    }
}

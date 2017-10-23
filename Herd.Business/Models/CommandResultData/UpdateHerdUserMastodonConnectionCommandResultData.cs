using Herd.Data.Models;

namespace Herd.Business.Models.CommandResultData
{
    public class UpdateHerdUserMastodonConnectionCommandResultData : CommandResultDataObject
    {
        public UserAccount User { get; set; }
    }
}

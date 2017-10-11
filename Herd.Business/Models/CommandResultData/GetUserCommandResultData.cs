using Herd.Data.Models;

namespace Herd.Business.Models.CommandResultData
{
    public class GetUserCommandResultData : CommandResultDataObject
    {
        public UserAccount User { get; set; }
    }
}
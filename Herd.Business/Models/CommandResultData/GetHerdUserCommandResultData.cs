using Herd.Data.Models;

namespace Herd.Business.Models.CommandResultData
{
    public class GetHerdUserCommandResultData : CommandResultDataObject
    {
        public UserAccount User { get; set; }
    }
}
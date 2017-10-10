using Herd.Data.Models;

namespace Herd.Business.Models.CommandResultData
{
    public class GetUserCommandResultData : CommandResultData
    {
        public UserAccount User { get; set; }
    }
}
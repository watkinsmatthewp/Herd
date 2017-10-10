using Herd.Data.Models;

namespace Herd.Business.Models.CommandResultData
{
    public class GetUserCommandResultData : CommandResultData
    {
        public HerdUserAccountDataModel User { get; set; }
    }
}
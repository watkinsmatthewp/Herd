using Herd.Data.Models;

namespace Herd.Business.Models.CommandResultData
{
    public class HerdAppGetUserCommandResultData : HerdAppCommandResultData
    {
        public HerdUserAccountDataModel User { get; set; }
    }
}
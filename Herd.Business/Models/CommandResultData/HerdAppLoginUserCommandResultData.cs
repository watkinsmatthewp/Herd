using Herd.Data.Models;

namespace Herd.Business.Models.CommandResultData
{
    public class HerdAppLoginUserCommandResultData : HerdAppCommandResultData
    {
        public HerdUserAccountDataModel User { get; set; }
    }
}
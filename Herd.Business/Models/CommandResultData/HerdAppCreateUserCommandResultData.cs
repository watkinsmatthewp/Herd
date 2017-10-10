using Herd.Data.Models;

namespace Herd.Business.Models.CommandResultData
{
    public class HerdAppCreateUserCommandResultData : HerdAppCommandResultData
    {
        public HerdUserAccountDataModel User { get; set; }
        public HerdUserProfileDataModel Profile { get; set; }
    }
}
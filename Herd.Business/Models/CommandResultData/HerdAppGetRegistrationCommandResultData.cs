using Herd.Data.Models;

namespace Herd.Business.Models.CommandResultData
{
    public class HerdAppGetRegistrationCommandResultData : HerdAppCommandResultData
    {
        public HerdAppRegistrationDataModel Registration { get; set; }
    }
}
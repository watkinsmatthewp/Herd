using Herd.Data.Models;

namespace Herd.Business.Models.CommandResultData
{
    public class GetRegistrationCommandResultData : CommandResultData
    {
        public HerdAppRegistrationDataModel Registration { get; set; }
    }
}
using Herd.Data.Models;

namespace Herd.Business.Models.CommandResultData
{
    public class GetRegistrationCommandResultData : CommandResultDataObject
    {
        public Registration Registration { get; set; }
    }
}
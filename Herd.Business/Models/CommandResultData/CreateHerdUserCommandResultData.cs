using Herd.Data.Models;

namespace Herd.Business.Models.CommandResultData
{
    public class CreateHerdUserCommandResultData : CommandResultDataObject
    {
        public UserAccount User { get; set; }
    }
}
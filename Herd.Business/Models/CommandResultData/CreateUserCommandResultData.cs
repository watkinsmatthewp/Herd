using Herd.Data.Models;

namespace Herd.Business.Models.CommandResultData
{
    public class CreateUserCommandResultData : CommandResultDataObject
    {
        public UserAccount User { get; set; }
    }
}
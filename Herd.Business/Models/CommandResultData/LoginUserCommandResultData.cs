using Herd.Data.Models;

namespace Herd.Business.Models.CommandResultData
{
    public class LoginUserCommandResultData : CommandResultDataObject
    {
        public UserAccount User { get; set; }
    }
}
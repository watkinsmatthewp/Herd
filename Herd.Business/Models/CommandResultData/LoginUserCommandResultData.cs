using Herd.Data.Models;

namespace Herd.Business.Models.CommandResultData
{
    public class LoginUserCommandResultData : CommandResultData
    {
        public UserAccount User { get; set; }
    }
}
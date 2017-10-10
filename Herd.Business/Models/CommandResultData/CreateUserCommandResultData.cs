using Herd.Data.Models;

namespace Herd.Business.Models.CommandResultData
{
    public class CreateUserCommandResultData : CommandResultData
    {
        public UserAccount User { get; set; }
        public UserProfile Profile { get; set; }
    }
}
using Herd.Data.Models;

namespace Herd.Business.Models.CommandResultData
{
    public class LoginHerdUserCommandResultData : CommandResultDataObject
    {
        public UserAccount User { get; set; }
    }
}
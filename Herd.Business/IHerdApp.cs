using Herd.Business.Models;
using Herd.Business.Models.CommandResultData;
using Herd.Business.Models.Commands;
using Herd.Data.Providers;
using System.Threading.Tasks;

namespace Herd.Business
{
    public interface IHerdApp
    {
        // Basic CRUD
        HerdAppCommandResult<HerdAppGetUserCommandResultData> GetUser(HerdAppGetUserCommand getUserCommand);

        // App registration
        HerdAppCommandResult<HerdAppGetRegistrationCommandResultData> GetRegistration(HerdAppGetRegistrationCommand getRegistrationCommand);
        HerdAppCommandResult<HerdAppGetRegistrationCommandResultData> GetOrCreateRegistration(HerdAppGetOrCreateRegistrationCommand getOrCreateRegistrationCommand);
        HerdAppCommandResult<HerdAppGetOAuthURLCommandResultData> GetOAuthURL(HerdAppGetOAuthURLCommand getOAuthUrlCommand);

        // Auth
        HerdAppCommandResult<HerdAppCreateUserCommandResultData> CreateUser(HerdAppCreateUserCommand createUserCommand);
        HerdAppCommandResult<HerdAppLoginUserCommandResultData> LoginUser(HerdAppLoginUserCommand loginUserCommand);

        // Feed
        HerdAppCommandResult<HerdAppGetRecentFeedItemsCommandResultData> GetRecentFeedItems(HerdAppGetRecentFeedItemsCommand getRecentFeedItemsCommand);
    }
}
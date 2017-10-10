using Herd.Business.Models;
using Herd.Business.Models.CommandResultData;
using Herd.Business.Models.Commands;

namespace Herd.Business
{
    public interface IHerdApp
    {
        // Basic CRUD
        HerdAppCommandResult<GetUserCommandResultData> GetUser(HerdAppGetUserCommand getUserCommand);

        // App registration
        HerdAppCommandResult<GetRegistrationCommandResultData> GetRegistration(HerdAppGetRegistrationCommand getRegistrationCommand);

        HerdAppCommandResult<GetRegistrationCommandResultData> GetOrCreateRegistration(HerdAppGetOrCreateRegistrationCommand getOrCreateRegistrationCommand);

        HerdAppCommandResult<GetOAuthURLCommandResultData> GetOAuthURL(HerdAppGetOAuthURLCommand getOAuthUrlCommand);

        // Auth
        HerdAppCommandResult<CreateUserCommandResultData> CreateUser(HerdAppCreateUserCommand createUserCommand);

        HerdAppCommandResult<LoginUserCommandResultData> LoginUser(HerdAppLoginUserCommand loginUserCommand);

        HerdAppCommandResult UpdateUserMastodonConnection(HerdAppUpdateUserMastodonConnectionCommand updateUserMastodonConnectionCommand);

        // Feed
        HerdAppCommandResult<GetRecentFeedItemsCommandResultData> GetRecentFeedItems(HerdAppGetRecentFeedItemsCommand getRecentFeedItemsCommand);

        HerdAppCommandResult<GetStatusCommandResultData> GetStatus(HerdAppGetStatusCommand getStatusCommand);

        HerdAppCommandResult CreateNewPost(HerdAppCreateNewPostCommand createNewPostCommand);
    }
}
using Herd.Business.Models;
using Herd.Business.Models.CommandResultData;
using Herd.Business.Models.Commands;

namespace Herd.Business
{
    public interface IHerdApp
    {
        // Basic CRUD
        HerdAppCommandResult<GetUserCommandResultData> GetUser(GetUserCommand getUserCommand);

        // App registration
        HerdAppCommandResult<GetRegistrationCommandResultData> GetRegistration(GetRegistrationCommand getRegistrationCommand);

        HerdAppCommandResult<GetRegistrationCommandResultData> GetOrCreateRegistration(GetOrCreateRegistrationCommand getOrCreateRegistrationCommand);

        HerdAppCommandResult<GetOAuthURLCommandResultData> GetOAuthURL(GetOAuthURLCommand getOAuthUrlCommand);

        // Auth
        HerdAppCommandResult<CreateUserCommandResultData> CreateUser(CreateUserCommand createUserCommand);

        HerdAppCommandResult<LoginUserCommandResultData> LoginUser(LoginUserCommand loginUserCommand);

        HerdAppCommandResult UpdateUserMastodonConnection(UpdateUserMastodonConnectionCommand updateUserMastodonConnectionCommand);

        // Feed
        HerdAppCommandResult<GetRecentFeedItemsCommandResultData> GetRecentFeedItems(GetRecentFeedItemsCommand getRecentFeedItemsCommand);

        HerdAppCommandResult<GetStatusCommandResultData> GetStatus(GetStatusCommand getStatusCommand);

        HerdAppCommandResult CreateNewPost(CreateNewPostCommand createNewPostCommand);
    }
}
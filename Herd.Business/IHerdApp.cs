using Herd.Business.Models;
using Herd.Business.Models.CommandResultData;
using Herd.Business.Models.Commands;
using Herd.Business.Models.Entities;

namespace Herd.Business
{
    public interface IHerdApp
    {
        // Basic CRUD
        CommandResult<GetUserCommandResultData> GetUser(GetUserCommand getUserCommand);

        // App registration
        CommandResult<GetRegistrationCommandResultData> GetRegistration(GetRegistrationCommand getRegistrationCommand);

        CommandResult<GetRegistrationCommandResultData> GetOrCreateRegistration(GetOrCreateRegistrationCommand getOrCreateRegistrationCommand);
        
        CommandResult<GetOAuthURLCommandResultData> GetOAuthURL(GetOAuthURLCommand getOAuthUrlCommand);

        // Auth
        CommandResult<CreateUserCommandResultData> CreateUser(CreateUserCommand createUserCommand);

        CommandResult<LoginUserCommandResultData> LoginUser(LoginUserCommand loginUserCommand);

        CommandResult<UpdateUserMastodonConnectionCommandResultData> UpdateUserMastodonConnection(UpdateUserMastodonConnectionCommand updateUserMastodonConnectionCommand);

        // Mastodon users
        CommandResult<SearchMastodonUsersCommandResultData> SearchUsers(SearchMastodonUsersCommand searchMastodonUsersCommand);
        CommandResult FollowUser(FollowUserCommand followUserCommand);

        // Mastodon posts
        CommandResult<SearchMastodonPostsCommandResultData> SearchPosts(SearchMastodonPostsCommand searchMastodonPostsCommand);
        CommandResult CreateNewPost(CreateNewPostCommand createNewPostCommand);
    }
}
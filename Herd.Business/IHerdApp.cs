using Herd.Business.Models;
using Herd.Business.Models.CommandResultData;
using Herd.Business.Models.Commands;
using Herd.Business.Models.Entities;

namespace Herd.Business
{
    public interface IHerdApp
    {
        // Basic CRUD
        CommandResult<GetHerdUserCommandResultData> GetUser(GetHerdUserCommand getUserCommand);

        // App registration
        CommandResult<GetRegistrationCommandResultData> GetRegistration(GetRegistrationCommand getRegistrationCommand);

        CommandResult<GetRegistrationCommandResultData> GetOrCreateRegistration(GetOrCreateRegistrationCommand getOrCreateRegistrationCommand);
        
        CommandResult<GetMastodonOAuthURLCommandResultData> GetOAuthURL(GetMastodonOAuthURLCommand getOAuthUrlCommand);

        // Auth
        CommandResult<CreateHerdUserCommandResultData> CreateUser(CreateHerdUserCommand createUserCommand);

        CommandResult<LoginHerdUserCommandResultData> LoginUser(LoginHerdUserCommand loginUserCommand);

        CommandResult<UpdateHerdUserMastodonConnectionCommandResultData> UpdateUserMastodonConnection(UpdateHerdUserMastodonConnectionCommand updateUserMastodonConnectionCommand);

        // Mastodon users
        CommandResult<SearchMastodonUsersCommandResultData> SearchUsers(SearchMastodonUsersCommand searchMastodonUsersCommand);
        CommandResult FollowUser(FollowMastodonUserCommand followUserCommand);

        // Mastodon posts
        CommandResult<SearchMastodonPostsCommandResultData> SearchPosts(SearchMastodonPostsCommand searchMastodonPostsCommand);
        CommandResult CreateNewPost(CreateNewMastodonPostCommand createNewPostCommand);
        CommandResult LikePost(LikeMastodonPostCommand likeCommand);
        CommandResult RepostPost(RepostMastodonPostCommand repostCommand);
    }
}
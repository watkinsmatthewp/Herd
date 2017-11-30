using Herd.Business.Models;
using Herd.Business.Models.CommandResultData;
using Herd.Business.Models.Commands;

namespace Herd.Business
{
    public interface IHerdApp
    {
        // App registration
        CommandResult<GetRegistrationCommandResultData> GetRegistration(GetRegistrationCommand getRegistrationCommand);

        CommandResult<GetRegistrationCommandResultData> GetOrCreateRegistration(GetOrCreateRegistrationCommand getOrCreateRegistrationCommand);

        CommandResult<GetMastodonOAuthURLCommandResultData> GetOAuthURL(GetMastodonOAuthURLCommand getOAuthUrlCommand);

        // Auth
        CommandResult<GetHerdUserCommandResultData> GetUser(GetHerdUserCommand getUserCommand);
        CommandResult<CreateHerdUserCommandResultData> CreateUser(CreateHerdUserCommand createUserCommand);

        CommandResult<LoginHerdUserCommandResultData> LoginUser(LoginHerdUserCommand loginUserCommand);

        CommandResult<UpdateHerdUserMastodonConnectionCommandResultData> UpdateUserMastodonConnection(UpdateHerdUserMastodonConnectionCommand updateUserMastodonConnectionCommand);

        // Mastodon users
        CommandResult<SearchMastodonUsersCommandResultData> SearchUsers(SearchMastodonUsersCommand searchMastodonUsersCommand);

        CommandResult FollowUser(FollowMastodonUserCommand followUserCommand);

        CommandResult UpdateUserMastodonProfile(UpdateUserMastodonProfileCommand updateMastodonProfileCommand);

        // Mastodon posts
        CommandResult<SearchMastodonPostsCommandResultData> SearchPosts(SearchMastodonPostsCommand searchMastodonPostsCommand);

        CommandResult CreateNewPost(CreateNewMastodonPostCommand createNewPostCommand);

        CommandResult DeletePost(DeleteMastodonPostCommand deleteComment);

        CommandResult LikePost(LikeMastodonPostCommand likeCommand);

        CommandResult RepostPost(RepostMastodonPostCommand repostCommand);

        // HashTags
        CommandResult<GetTopHashTagsCommandResultData> GetTopHashTags(GetTopHashTagsCommand getTopHashTagsCommand);
        CommandResult CreateTopHashTags();
    }
}
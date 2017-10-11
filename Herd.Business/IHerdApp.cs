﻿using Herd.Business.Models;
using Herd.Business.Models.CommandResultData;
using Herd.Business.Models.Commands;

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

        CommandResult UpdateUserMastodonConnection(UpdateUserMastodonConnectionCommand updateUserMastodonConnectionCommand);

        // Feed
        CommandResult<GetRecentFeedItemsCommandResultData> GetRecentFeedItems(GetRecentFeedItemsCommand getRecentFeedItemsCommand);

        CommandResult<GetStatusCommandResultData> GetStatus(GetStatusCommand getStatusCommand);

        CommandResult CreateNewPost(CreateNewPostCommand createNewPostCommand);

        // Mastodon users
        CommandResult<SearchMastodonUsersCommandResultData> SearchUsers(SearchMastodonUsersCommand searchMastodonUsersCommand);
    }
}
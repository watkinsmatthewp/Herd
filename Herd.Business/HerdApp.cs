using Herd.Business.Models;
using Herd.Business.ApiWrappers;
using Herd.Business.Models.CommandResultData;
using Herd.Business.Models.Commands;
using Herd.Business.Models.Entities;
using Herd.Core;
using Herd.Data.Models;
using Herd.Data.Providers;
using Herd.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Herd.Core.Exceptions;
using Herd.Core.Errors;
using System.Threading.Tasks;
using Herd.Business.Extensions;

namespace Herd.Business
{
    public class HerdApp : IHerdApp
    {
        private const string NON_REDIRECT_URL = "urn:ietf:wg:oauth:2.0:oob";

        private static Random _saltGenerator = new Random(Guid.NewGuid().GetHashCode());

        private IDataProvider _data;
        private IMastodonApiWrapper _mastodonApiWrapper;
        private ILogger _logger;

        public HerdApp(IDataProvider data, IMastodonApiWrapper mastodonApiWrapper, ILogger logger)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            _mastodonApiWrapper = mastodonApiWrapper ?? throw new ArgumentNullException(nameof(mastodonApiWrapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region App registration

        public CommandResult<GetRegistrationCommandResultData> GetRegistration(GetRegistrationCommand getRegistrationCommand)
        {
            return ProcessCommand<GetRegistrationCommandResultData>(result =>
            {
                result.Data = new GetRegistrationCommandResultData
                {
                    Registration = _data.GetAppRegistration(getRegistrationCommand.ID)
                };
            });
        }

        public CommandResult<GetOAuthURLCommandResultData> GetOAuthURL(GetOAuthURLCommand getOAuthUrlCommand)
        {
            return ProcessCommand<GetOAuthURLCommandResultData>(result =>
            {
                var returnURL = string.IsNullOrWhiteSpace(getOAuthUrlCommand.ReturnURL) ? NON_REDIRECT_URL : getOAuthUrlCommand.ReturnURL;
                _mastodonApiWrapper.AppRegistration = _data.GetAppRegistration(getOAuthUrlCommand.AppRegistrationID) ?? throw new UserErrorException("No app registration with that ID");
                result.Data = new GetOAuthURLCommandResultData
                {
                    URL = _mastodonApiWrapper.GetOAuthUrl(getOAuthUrlCommand.ReturnURL)
                };
            });
        }

        public CommandResult<GetRegistrationCommandResultData> GetOrCreateRegistration(GetOrCreateRegistrationCommand getOrCreateRegistrationCommand)
        {
            return ProcessCommand<GetRegistrationCommandResultData>(result =>
            {
                result.Data = new GetRegistrationCommandResultData
                {
                    Registration = _data.GetAppRegistration(getOrCreateRegistrationCommand.Instance)
                        ?? _data.CreateAppRegistration(new MastodonApiWrapper(getOrCreateRegistrationCommand.Instance).RegisterApp().Synchronously())
                };
            });
        }

        #endregion App registration

        #region Users

        public CommandResult<GetUserCommandResultData> GetUser(GetUserCommand getUserCommand)
        {
            return ProcessCommand<GetUserCommandResultData>(result =>
            {
                result.Data = new GetUserCommandResultData
                {
                    User = _data.GetUser(getUserCommand.UserID)
                };
            });
        }

        public CommandResult<CreateUserCommandResultData> CreateUser(CreateUserCommand createUserCommand)
        {
            return ProcessCommand<CreateUserCommandResultData>(result =>
            {
                var userByEmail = _data.GetUser(createUserCommand.Email);
                if (userByEmail != null)
                {
                    throw new UserErrorException("That email address has already been taken");
                }

                var saltKey = _saltGenerator.Next();
                result.Data = new CreateUserCommandResultData
                {
                    User = _data.CreateUser(new UserAccount
                    {
                        Email = createUserCommand.Email,
                        Security = new UserAccountSecurity
                        {
                            SaltKey = saltKey,
                            SaltedPassword = createUserCommand.PasswordPlainText.Hashed(saltKey)
                        }
                    })
                };
            });
        }

        public CommandResult<LoginUserCommandResultData> LoginUser(LoginUserCommand loginUserCommand)
        {
            return ProcessCommand<LoginUserCommandResultData>(result =>
            {
                var userByEmail = _data.GetUser(loginUserCommand.Email);
                if (userByEmail?.PasswordIs(loginUserCommand.PasswordPlainText) != true)
                {
                    throw new UserErrorException("Wrong email or password");
                }
                result.Data = new LoginUserCommandResultData
                {
                    User = userByEmail
                };
            });
        }

        public CommandResult UpdateUserMastodonConnection(UpdateUserMastodonConnectionCommand updateUserMastodonConnectionCommand)
        {
            return ProcessCommand(result =>
            {
                // Check the token
                if (string.IsNullOrWhiteSpace(updateUserMastodonConnectionCommand.Token))
                {
                    throw new UserErrorException("Token cannot be empty");
                }

                // Check the user ID
                var user = _data.GetUser(updateUserMastodonConnectionCommand.UserID);
                if (user == null)
                {
                    throw new UserErrorException("Could not find a user with that ID");
                }

                // Connect with the one-time use code to get the permanent code
                _mastodonApiWrapper.AppRegistration = _data.GetAppRegistration(updateUserMastodonConnectionCommand.AppRegistrationID)
                    ?? throw new UserErrorException("Could not find a registration with that ID");
                _mastodonApiWrapper.UserMastodonConnectionDetails = _mastodonApiWrapper.Connect(updateUserMastodonConnectionCommand.Token).Synchronously();

                // Update the user details
                user.MastodonConnection = _mastodonApiWrapper.UserMastodonConnectionDetails;
                _data.UpdateUser(user);
            });
        }

        #endregion Users

        #region Feed

        public CommandResult<GetRecentPostsCommandResultData> GetRecentFeedItems(GetRecentPostsCommand getRecentFeedItemsCommand)
        {
            return ProcessCommand<GetRecentPostsCommandResultData>(result =>
            {
                result.Data = new GetRecentPostsCommandResultData
                {
                    RecentPosts = _mastodonApiWrapper.GetRecentPosts(
                        getRecentFeedItemsCommand.IncludeInReplyToPost,
                        getRecentFeedItemsCommand.IncludeReplyPosts,
                        getRecentFeedItemsCommand.MaxPostID,
                        getRecentFeedItemsCommand.SincePostID,
                        getRecentFeedItemsCommand.MaxCount
                    ).Synchronously()
                };
            });
        }

        public CommandResult<GetPostCommandResultData> GetStatus(GetPostCommand getStatusCommand)
        {
            return ProcessCommand<GetPostCommandResultData>(result =>
            {
                result.Data = new GetPostCommandResultData
                {
                    MastodonPost = _mastodonApiWrapper.GetPost(getStatusCommand.PostID, getStatusCommand.IncludeAncestors, getStatusCommand.IncludeDescendants).Synchronously()
                };
            });
        }

        public CommandResult CreateNewPost(CreateNewPostCommand createNewPostCommand)
        {
            return ProcessCommand(result =>
            {
                _mastodonApiWrapper.CreateNewPost(
                    createNewPostCommand.Message, createNewPostCommand.Visibility,
                    createNewPostCommand.ReplyStatusId, createNewPostCommand.MediaIds,
                    createNewPostCommand.Sensitive, createNewPostCommand.SpoilerText
                ).Synchronously();
            });
        }

        #endregion Feed

        #region Mastodon Users

        public CommandResult<SearchMastodonUsersCommandResultData> SearchUsers(SearchMastodonUsersCommand searchMastodonUsersCommand)
        {
            return ProcessCommand<SearchMastodonUsersCommandResultData>(result =>
            {
                if (searchMastodonUsersCommand.IsGlobalSearch)
                {
                    throw new UserErrorException("Please specify at least one search criterion");
                }

                result.Data = new SearchMastodonUsersCommandResultData
                {
                    Users = GetUsers(searchMastodonUsersCommand).Synchronously()
                };
            });
        }

        private async Task<List<MastodonUser>> GetUsers(SearchMastodonUsersCommand searchMastodonUsersCommand)
        {
            var users = null as Dictionary<int, MastodonUser>;

            if (searchMastodonUsersCommand.UserID.HasValue)
            {
                users = await FilterByUserID(users, searchMastodonUsersCommand.UserID.Value);
            }
            if (searchMastodonUsersCommand.FollowedByUserID.HasValue)
            {
                users = await FilterByFollowedByUserID(users, searchMastodonUsersCommand.FollowedByUserID.Value, searchMastodonUsersCommand.MaxCount);
            }
            if (searchMastodonUsersCommand.FollowsUserID.HasValue)
            {
                users = await FilterByFollowsByUserID(users, searchMastodonUsersCommand.FollowsUserID.Value, searchMastodonUsersCommand.MaxCount);
            }
            if (!string.IsNullOrWhiteSpace(searchMastodonUsersCommand.Name))
            {
                users = await FilterByName(users, searchMastodonUsersCommand.Name, searchMastodonUsersCommand.MaxCount);
            }

            var usersList = users.Values.ToList();
            users = null;
            for (var i = 0; i < usersList.Count; i++)
            {
                usersList[i] = await _mastodonApiWrapper.AddContextToMastodonUser(usersList[i], searchMastodonUsersCommand.IncludeFollowers, searchMastodonUsersCommand.IncludeFollowing);
            }

            return usersList;
        }

        private async Task<Dictionary<int, MastodonUser>> FilterByUserID(Dictionary<int, MastodonUser> userSet1, int mastodonUserID)
        {
            if (userSet1?.Count == 0)
            {
                return new Dictionary<int, MastodonUser>();
            }
            var mastodonAccount = await _mastodonApiWrapper.GetMastodonAccount(mastodonUserID);
            var newUserSet = mastodonAccount == null ? new MastodonUser[0] : new[] { mastodonAccount };
            return Filter(userSet1, newUserSet);
        }

        private async Task<Dictionary<int, MastodonUser>> FilterByName(Dictionary<int, MastodonUser> userSet1, string name, int limit)
        {
            if (userSet1?.Count == 0)
            {
                return new Dictionary<int, MastodonUser>();
            }
            return Filter(userSet1, await _mastodonApiWrapper.GetUsersByName(name, false, false, limit));
        }

        private async Task<Dictionary<int, MastodonUser>> FilterByFollowedByUserID(Dictionary<int, MastodonUser> userSet1, int followedByUserID, int limit)
        {
            if (userSet1?.Count == 0)
            {
                return new Dictionary<int, MastodonUser>();
            }
            return Filter(userSet1, await _mastodonApiWrapper.GetFollowing(followedByUserID, false, false, limit));
        }

        private async Task<Dictionary<int, MastodonUser>> FilterByFollowsByUserID(Dictionary<int, MastodonUser> userSet1, int followedUserID, int limit)
        {
            if (userSet1?.Count == 0)
            {
                return new Dictionary<int, MastodonUser>();
            }
            return Filter(userSet1, await _mastodonApiWrapper.GetFollowers(followedUserID, false, false, limit));
        }

        private Dictionary<int, MastodonUser> Filter(Dictionary<int, MastodonUser> userSet1, IList<MastodonUser> userSet2)
        {
            if (userSet1 == null)
            {
                return ToDictionary(userSet2);
            }
            if (userSet1.Count == 0)
            {
                return new Dictionary<int, MastodonUser>();
            }
            var idsToPreserve = new HashSet<int>(userSet1.Keys.Intersect(userSet2.Select(u => u.MastodonUserId)));
            return ToDictionary(userSet2.Where(u => idsToPreserve.Contains(u.MastodonUserId)));
        }

        private Dictionary<int, MastodonUser> ToDictionary(IEnumerable<MastodonUser> userSet)
        {
            return userSet.ToDictionary(u => u.MastodonUserId, u => u);
        }

        #endregion Mastodon Users

        #region Private helpers

        private CommandResult<D> ProcessCommand<D>(Action<CommandResult<D>> doWork)
            where D : CommandResultDataObject
        {
            return ProcessCommand(new CommandResult<D>(), doWork);
        }

        private CommandResult ProcessCommand(Action<CommandResult> doWork)
        {
            return ProcessCommand(new CommandResult(), doWork);
        }

        private R ProcessCommand<R>(R result, Action<R> doWork) where R : CommandResult
        {
            try
            {
                doWork(result);
            }
            catch (Exception e)
            {
                var errorID = Guid.NewGuid();
                _logger.Error(errorID, "Error in HerdApp", null, e);
                result.Errors.Add((e as ErrorException)?.Error ?? new SystemError
                {
                    Message = $"Unhandled exception: {e.Message}"
                });
            }
            return result;
        }

        #endregion Private helpers
    }
}
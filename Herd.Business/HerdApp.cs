﻿using Herd.Business.Models;
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

        public CommandResult<UpdateUserMastodonConnectionCommandResultData> UpdateUserMastodonConnection(UpdateUserMastodonConnectionCommand updateUserMastodonConnectionCommand)
        {
            return ProcessCommand<UpdateUserMastodonConnectionCommandResultData>(result =>
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

                result.Data = new UpdateUserMastodonConnectionCommandResultData
                {
                    User = _data.GetUser(user.ID)
                };
            });
        }

        public CommandResult<GetUserCommandResultData> GetFollowing(GetUserCommand getUserCommand)
        {
            return ProcessCommand<GetUserCommandResultData>(result =>
            {
                var Following = _mastodonApiWrapper.GetFollowing(
                    getUserCommand.UserID
                ).Synchronously();
            }
            );
        }

        #endregion Users

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
            var users = null as Dictionary<string, MastodonUser>;

            if (!string.IsNullOrWhiteSpace(searchMastodonUsersCommand.UserID))
            {
                users = await FilterByUserID(users, searchMastodonUsersCommand.UserID);
            }
            if (!string.IsNullOrWhiteSpace(searchMastodonUsersCommand.FollowedByUserID))
            {
                users = await FilterByFollowedByUserID(users, searchMastodonUsersCommand.FollowedByUserID, searchMastodonUsersCommand.MaxCount);
            }
            if (!string.IsNullOrWhiteSpace(searchMastodonUsersCommand.FollowsUserID))
            {
                users = await FilterByFollowsByUserID(users, searchMastodonUsersCommand.FollowsUserID, searchMastodonUsersCommand.MaxCount);
            }
            if (!string.IsNullOrWhiteSpace(searchMastodonUsersCommand.Name))
            {
                users = await FilterByName(users, searchMastodonUsersCommand.Name, searchMastodonUsersCommand.MaxCount);
            }

            return await _mastodonApiWrapper.AddContextToMastodonUsers
            (
                users.Values.ToList(),
                searchMastodonUsersCommand.IncludeFollowers,
                searchMastodonUsersCommand.IncludeFollowing,
                searchMastodonUsersCommand.IncludeFollowedByActiveUser,
                searchMastodonUsersCommand.IncludeFollowsActiveUser
            );
        }

        private async Task<Dictionary<string, MastodonUser>> FilterByUserID(Dictionary<string, MastodonUser> userSet1, string mastodonUserID)
        {
            if (userSet1?.Count == 0)
            {
                return new Dictionary<string, MastodonUser>();
            }
            return await Filter(userSet1, () => GetMastodonUserOrEmptySet(mastodonUserID), u => u.MastodonUserId);
        }

        private async Task<IList<MastodonUser>> GetMastodonUserOrEmptySet(string mastodonUserID)
        {
            var mastodonAccount = await _mastodonApiWrapper.GetMastodonAccount(mastodonUserID);
            return mastodonAccount == null ? new MastodonUser[0] : new[] { mastodonAccount };
        }

        private async Task<Dictionary<string, MastodonUser>> FilterByName(Dictionary<string, MastodonUser> userSet1, string name, int limit)
        {
            if (userSet1?.Count == 0)
            {
                return new Dictionary<string, MastodonUser>();
            }
            return await Filter(userSet1, () => _mastodonApiWrapper.GetUsersByName(name, false, false, false, false, limit), u => u.MastodonUserId);
        }

        private async Task<Dictionary<string, MastodonUser>> FilterByFollowedByUserID(Dictionary<string, MastodonUser> userSet1, string followedByUserID, int limit)
        {
            if (userSet1?.Count == 0)
            {
                return new Dictionary<string, MastodonUser>();
            }
            return await Filter(userSet1, () => _mastodonApiWrapper.GetFollowing(followedByUserID, false, false, false, false, limit), u => u.MastodonUserId);
        }

        private async Task<Dictionary<string, MastodonUser>> FilterByFollowsByUserID(Dictionary<string, MastodonUser> userSet1, string followedUserID, int limit)
        {
            if (userSet1?.Count == 0)
            {
                return new Dictionary<string, MastodonUser>();
            }
            return await Filter(userSet1, () => _mastodonApiWrapper.GetFollowers(followedUserID, false, false, false, false, limit), u => u.MastodonUserId);
        }

        public CommandResult FollowUser(FollowUserCommand followUserCommand)
        {
            return ProcessCommand(result =>
            {
                _mastodonApiWrapper.Follow(followUserCommand.UserID, followUserCommand.FollowUser);
            });
        }

        #endregion Mastodon Users

        #region Mastodon Posts

        public CommandResult<SearchMastodonPostsCommandResultData> SearchPosts(SearchMastodonPostsCommand searchMastodonPostsCommand)
        {
            return ProcessCommand<SearchMastodonPostsCommandResultData>(result =>
            {
                if (searchMastodonPostsCommand.IsGlobalSearch)
                {
                    throw new UserErrorException("Please specify at least one search criterion");
                }

                result.Data = new SearchMastodonPostsCommandResultData
                {
                    Posts = GetPosts(searchMastodonPostsCommand).Synchronously()
                };
            });
        }

        private async Task<List<MastodonPost>> GetPosts(SearchMastodonPostsCommand searchMastodonPostsCommand)
        {
            var posts = null as Dictionary<string, MastodonPost>;

            if (!string.IsNullOrWhiteSpace(searchMastodonPostsCommand.PostID))
            {
                posts = await FilterByPostID(posts, searchMastodonPostsCommand.PostID);
            }
            if (!string.IsNullOrWhiteSpace(searchMastodonPostsCommand.ByAuthorMastodonUserID))
            {
                posts = await FilterByAuthorUserID(posts, searchMastodonPostsCommand.ByAuthorMastodonUserID);
            }
            if (searchMastodonPostsCommand.OnlyOnlyOnActiveUserTimeline)
            {
                posts = await FilterByOnActiveUserTimeline(posts);
            }
            if (!string.IsNullOrWhiteSpace(searchMastodonPostsCommand.HavingHashTag))
            {
                posts = await FilterByHashTag(posts, searchMastodonPostsCommand.HavingHashTag);
            }

            return posts.Values.ToList();
        }

        private Task<Dictionary<string, MastodonPost>> FilterByPostID(Dictionary<string, MastodonPost> posts, string postID)
        {
            throw new NotImplementedException();
        }

        private Task<Dictionary<string, MastodonPost>> FilterByAuthorUserID(Dictionary<string, MastodonPost> posts, string byAuthorMastodonUserID)
        {
            throw new NotImplementedException();
        }

        private Task<Dictionary<string, MastodonPost>> FilterByOnActiveUserTimeline(Dictionary<string, MastodonPost> posts)
        {
            throw new NotImplementedException();
        }

        private Task<Dictionary<string, MastodonPost>> FilterByHashTag(Dictionary<string, MastodonPost> posts, string havingHashTag)
        {
            throw new NotImplementedException();
        }

        public CommandResult CreateNewPost(CreateNewPostCommand createNewPostCommand)
        {
            return ProcessCommand(result =>
            {
                _mastodonApiWrapper.CreateNewPost(
                    createNewPostCommand.Message,
                    createNewPostCommand.Visibility,
                    createNewPostCommand.ReplyStatusId,
                    createNewPostCommand.MediaIds,
                    createNewPostCommand.Sensitive,
                    createNewPostCommand.SpoilerText
                ).Synchronously();
            });
        }

        #endregion

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

        private async Task<Dictionary<string, T>> Filter<T>(Dictionary<string, T> set1, Func<Task<IList<T>>> getSet2, Func<T, string> getID)
        {
            if (set1 == null)
            {
                return ToDictionary(await getSet2(), getID);
            }
            if (set1.Count == 0)
            {
                return new Dictionary<string, T>();
            }
            var set2 = await getSet2();
            var idsToPreserve = new HashSet<string>(set1.Keys.Intersect(set2.Select(getID)));
            return ToDictionary(set2.Where(u => idsToPreserve.Contains(getID(u))), getID);
        }

        private Dictionary<string, T> ToDictionary<T>(IEnumerable<T> set, Func<T, string> getID)
        {
            return set.ToDictionary(getID, u => u);
        }

        #endregion Private helpers
    }
}
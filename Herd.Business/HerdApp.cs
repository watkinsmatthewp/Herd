using Herd.Business.ApiWrappers;
using Herd.Business.ApiWrappers.MastodonObjectContextOptions;
using Herd.Business.Extensions;
using Herd.Business.Models;
using Herd.Business.Models.CommandResultData;
using Herd.Business.Models.Commands;
using Herd.Business.Models.Entities;
using Herd.Core;
using Herd.Core.Errors;
using Herd.Core.Exceptions;
using Herd.Data.Models;
using Herd.Data.Providers;
using Herd.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public CommandResult<GetMastodonOAuthURLCommandResultData> GetOAuthURL(GetMastodonOAuthURLCommand getOAuthUrlCommand)
        {
            return ProcessCommand<GetMastodonOAuthURLCommandResultData>(result =>
            {
                var returnURL = string.IsNullOrWhiteSpace(getOAuthUrlCommand.ReturnURL) ? NON_REDIRECT_URL : getOAuthUrlCommand.ReturnURL;
                _mastodonApiWrapper.AppRegistration = _data.GetAppRegistration(getOAuthUrlCommand.AppRegistrationID) ?? throw new UserErrorException("No app registration with that ID");
                result.Data = new GetMastodonOAuthURLCommandResultData
                {
                    URL = _mastodonApiWrapper.GetOAuthUrl(getOAuthUrlCommand.ReturnURL)
                };
            });
        }

        public CommandResult<GetRegistrationCommandResultData> GetOrCreateRegistration(GetOrCreateRegistrationCommand getOrCreateRegistrationCommand)
        {
            return ProcessCommand<GetRegistrationCommandResultData>(result =>
            {
                var registration = _data.GetAppRegistration(getOrCreateRegistrationCommand.Instance);
                if (registration == null)
                {
                    _mastodonApiWrapper.MastodonHostInstance = getOrCreateRegistrationCommand.Instance;
                    registration = _mastodonApiWrapper.RegisterApp().Synchronously();
                    registration = _data.CreateAppRegistration(registration);
                }

                result.Data = new GetRegistrationCommandResultData
                {
                    Registration = registration
                };
            });
        }

        #endregion App registration

        #region Users

        public CommandResult<GetHerdUserCommandResultData> GetUser(GetHerdUserCommand getUserCommand)
        {
            return ProcessCommand<GetHerdUserCommandResultData>(result =>
            {
                result.Data = new GetHerdUserCommandResultData
                {
                    User = _data.GetUser(getUserCommand.UserID)
                };
            });
        }

        public CommandResult<CreateHerdUserCommandResultData> CreateUser(CreateHerdUserCommand createUserCommand)
        {
            return ProcessCommand<CreateHerdUserCommandResultData>(result =>
            {
                var userByEmail = _data.GetUser(createUserCommand.Email);
                if (userByEmail != null)
                {
                    throw new UserErrorException("That email address has already been taken");
                }

                var saltKey = _saltGenerator.Next();
                result.Data = new CreateHerdUserCommandResultData
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

        public CommandResult<LoginHerdUserCommandResultData> LoginUser(LoginHerdUserCommand loginUserCommand)
        {
            return ProcessCommand<LoginHerdUserCommandResultData>(result =>
            {
                var userByEmail = _data.GetUser(loginUserCommand.Email);
                if (userByEmail?.PasswordIs(loginUserCommand.PasswordPlainText) != true)
                {
                    throw new UserErrorException("Wrong email or password");
                }
                result.Data = new LoginHerdUserCommandResultData
                {
                    User = userByEmail
                };
            });
        }

        public CommandResult<UpdateHerdUserMastodonConnectionCommandResultData> UpdateUserMastodonConnection(UpdateHerdUserMastodonConnectionCommand updateUserMastodonConnectionCommand)
        {
            return ProcessCommand<UpdateHerdUserMastodonConnectionCommandResultData>(result =>
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

                result.Data = new UpdateHerdUserMastodonConnectionCommandResultData
                {
                    User = _data.GetUser(user.ID)
                };
            });
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

        public CommandResult FollowUser(FollowMastodonUserCommand followUserCommand)
        {
            return ProcessCommand(result =>
            {
                _mastodonApiWrapper.Follow(followUserCommand.UserID, followUserCommand.FollowUser).Synchronously();
            });
        }

        #endregion Mastodon Users

        #region Mastodon Posts

        /// <summary>
        /// Processes a command to like a post
        /// </summary>
        /// <param name="likeCommand"></param>
        /// <returns></returns>
        public CommandResult LikePost(LikeMastodonPostCommand likeCommand)
        {
            return ProcessCommand(result =>
            {
                _mastodonApiWrapper.Like(likeCommand.PostID, likeCommand.Like).Synchronously();
            });
        }

        /// <summary>
        /// Processes command to repost a post
        /// </summary>
        /// <param name="repostCommand"></param>
        /// <returns></returns>
        public CommandResult RepostPost(RepostMastodonPostCommand repostCommand)
        {
            return ProcessCommand(result =>
            {
                _mastodonApiWrapper.Repost(repostCommand.PostID, repostCommand.Repost).Synchronously();
            });
        }

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

        public CommandResult CreateNewPost(CreateNewMastodonPostCommand createNewPostCommand)
        {
            return ProcessCommand(result =>
            {
                var mediaId = null as string;
                if (createNewPostCommand.AttachmentStream?.Length > 0)
                {
                    mediaId = _mastodonApiWrapper.UploadAttachment(createNewPostCommand.AttachmentStream).Synchronously().Id;
                }

                _mastodonApiWrapper.CreateNewPost(
                    createNewPostCommand.Message,
                    createNewPostCommand.Visibility,
                    createNewPostCommand.ReplyStatusId,
                    ( string.IsNullOrWhiteSpace(mediaId) ? new string[0] :  new string[] { mediaId } ),
                    createNewPostCommand.Sensitive,
                    createNewPostCommand.SpoilerText
                ).Synchronously();
            });
        }

        #endregion Mastodon Posts

        #region Private helpers

        #region Mastodon Users

        private async Task<IList<MastodonUser>> GetUsers(SearchMastodonUsersCommand searchMastodonUsersCommand)
        {
            var users = null as Dictionary<string, MastodonUser>;

            if (!string.IsNullOrWhiteSpace(searchMastodonUsersCommand.UserID))
            {
                users = await FilterByUserID(users, searchMastodonUsersCommand.UserID);
            }
            if (!string.IsNullOrWhiteSpace(searchMastodonUsersCommand.FollowedByUserID))
            {
                users = await FilterByFollowedByUserID(users, searchMastodonUsersCommand.FollowedByUserID, searchMastodonUsersCommand.PagingOptions);
            }
            if (!string.IsNullOrWhiteSpace(searchMastodonUsersCommand.FollowsUserID))
            {
                users = await FilterByFollowsByUserID(users, searchMastodonUsersCommand.FollowsUserID, searchMastodonUsersCommand.PagingOptions);
            }
            if (!string.IsNullOrWhiteSpace(searchMastodonUsersCommand.Name))
            {
                users = await FilterByName(users, searchMastodonUsersCommand.Name, searchMastodonUsersCommand.PagingOptions);
            }

            await _mastodonApiWrapper.AddContextToMastodonUsers
            (
                users.Values,
                new MastodonUserContextOptions
                {
                    IncludeFollowers = searchMastodonUsersCommand.IncludeFollowers,
                    IncludeFollowing = searchMastodonUsersCommand.IncludeFollowing,
                    IncludeIsFollowedByActiveUser = searchMastodonUsersCommand.IncludeFollowedByActiveUser,
                    IncludeFollowsActiveUser = searchMastodonUsersCommand.IncludeFollowsActiveUser
                }
            );

            return users.Values.ToArray();
        }

        private Task<Dictionary<string, MastodonUser>> FilterByUserID(Dictionary<string, MastodonUser> userSet1, string mastodonUserID)
        {
            return Filter(userSet1, () => GetMastodonUserOrEmptySet(mastodonUserID), u => u.MastodonUserId);
        }

        private async Task<IList<MastodonUser>> GetMastodonUserOrEmptySet(string mastodonUserID)
        {
            var mastodonAccount = await _mastodonApiWrapper.GetMastodonAccount(mastodonUserID);
            return mastodonAccount == null ? new MastodonUser[0] : new[] { mastodonAccount };
        }

        private Task<Dictionary<string, MastodonUser>> FilterByName(Dictionary<string, MastodonUser> userSet1, string name, PagingOptions pagingOptions)
        {
            return Filter(userSet1, () => _mastodonApiWrapper.GetUsersByName(name, null, pagingOptions), u => u.MastodonUserId);
        }

        private Task<Dictionary<string, MastodonUser>> FilterByFollowedByUserID(Dictionary<string, MastodonUser> userSet1, string followedByUserID, PagingOptions pagingOptions)
        {
            return Filter(userSet1, () => _mastodonApiWrapper.GetFollowing(followedByUserID, null, pagingOptions), u => u.MastodonUserId);
        }

        private Task<Dictionary<string, MastodonUser>> FilterByFollowsByUserID(Dictionary<string, MastodonUser> userSet1, string followedUserID, PagingOptions pagingOptions)
        {
            return Filter(userSet1, () => _mastodonApiWrapper.GetFollowers(followedUserID, null, pagingOptions), u => u.MastodonUserId);
        }

        #endregion Mastodon Users

        #region Mastodon Posts

        private async Task<IList<MastodonPost>> GetPosts(SearchMastodonPostsCommand searchMastodonPostsCommand)
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

            await _mastodonApiWrapper.AddContextToMastodonPosts
            (
                posts.Values,
                new MastodonPostContextOptions
                {
                    IncludeAncestors = searchMastodonPostsCommand.IncludeAncestors,
                    IncludeDescendants = searchMastodonPostsCommand.IncludeDescendants
                }
            );

            return posts.Values.ToArray();
        }

        private Task<Dictionary<string, MastodonPost>> FilterByPostID(Dictionary<string, MastodonPost> postSet1, string postID)
        {
            return Filter(postSet1, () => GetMastodonPostOrEmptySet(postID), p => p.Id);
        }

        private async Task<IList<MastodonPost>> GetMastodonPostOrEmptySet(string postID)
        {
            var post = await _mastodonApiWrapper.GetPost(postID);
            return post == null ? new MastodonPost[0] : new[] { post };
        }

        private Task<Dictionary<string, MastodonPost>> FilterByAuthorUserID(Dictionary<string, MastodonPost> postSet1, string byAuthorMastodonUserID)
        {
            return Filter(postSet1, () => _mastodonApiWrapper.GetPostsByAuthorUserID(byAuthorMastodonUserID), p => p.Id);
        }

        private Task<Dictionary<string, MastodonPost>> FilterByOnActiveUserTimeline(Dictionary<string, MastodonPost> postSet1)
        {
            return Filter(postSet1, () => _mastodonApiWrapper.GetPostsOnActiveUserTimeline(), p => p.Id);
        }

        private Task<Dictionary<string, MastodonPost>> FilterByHashTag(Dictionary<string, MastodonPost> postSet1, string hashTag)
        {
            return Filter(postSet1, () => _mastodonApiWrapper.GetPostsByHashTag(hashTag), p => p.Id);
        }

        #endregion Mastodon Posts

        #region Generic

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
            if (set1?.Count == 0)
            {
                return new Dictionary<string, T>();
            }
            var set2 = await getSet2();
            if (set1 == null)
            {
                return ToDictionary(set2, getID);
            }
            var idsToPreserve = new HashSet<string>(set1.Keys.Intersect(set2.Select(getID)));
            return ToDictionary(set2.Where(u => idsToPreserve.Contains(getID(u))), getID);
        }

        private Dictionary<string, T> ToDictionary<T>(IEnumerable<T> set, Func<T, string> getID)
        {
            return set.ToDictionary(getID, u => u);
        }

        #endregion Generic

        #endregion Private helpers
    }
}
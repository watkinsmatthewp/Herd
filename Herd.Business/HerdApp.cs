using Herd.Business.App.Exceptions;
using Herd.Business.Models;
using Herd.Business.Models.CommandResultData;
using Herd.Business.Models.Commands;
using Herd.Business.Models.Errors;
using Herd.Business.Models.MastodonWrappers;
using Herd.Core;
using Herd.Data.Models;
using Herd.Data.Providers;
using Herd.Logging;
using System;
using System.Linq;

namespace Herd.Business
{
    public class HerdApp : IHerdApp
    {
        private const string NON_REDIRECT_URL = "urn:ietf:wg:oauth:2.0:oob";

        private static Random _saltGenerator = new Random(Guid.NewGuid().GetHashCode());

        private IHerdDataProvider _data;
        private IMastodonApiWrapper _mastodonApiWrapper;
        private IHerdLogger _logger;

        public HerdApp(IHerdDataProvider data, IMastodonApiWrapper mastodonApiWrapper, IHerdLogger logger)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            _mastodonApiWrapper = mastodonApiWrapper ?? throw new ArgumentNullException(nameof(mastodonApiWrapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region App registration

        public HerdAppCommandResult<HerdAppGetRegistrationCommandResultData> GetRegistration(HerdAppGetRegistrationCommand getRegistrationCommand)
        {
            return ProcessCommand<HerdAppGetRegistrationCommandResultData>(result =>
            {
                result.Data = new HerdAppGetRegistrationCommandResultData
                {
                    Registration = _data.GetAppRegistration(getRegistrationCommand.ID)
                };
            });
        }

        public HerdAppCommandResult<HerdAppGetOAuthURLCommandResultData> GetOAuthURL(HerdAppGetOAuthURLCommand getOAuthUrlCommand)
        {
            return ProcessCommand<HerdAppGetOAuthURLCommandResultData>(result =>
            {
                var returnURL = string.IsNullOrWhiteSpace(getOAuthUrlCommand.ReturnURL) ? NON_REDIRECT_URL : getOAuthUrlCommand.ReturnURL;
                _mastodonApiWrapper.AppRegistration = _data.GetAppRegistration(getOAuthUrlCommand.AppRegistrationID) ?? throw new HerdAppUserErrorException("No app registration with that ID");
                result.Data = new HerdAppGetOAuthURLCommandResultData
                {
                    URL = _mastodonApiWrapper.GetOAuthUrl(getOAuthUrlCommand.ReturnURL)
                };
            });
        }

        public HerdAppCommandResult<HerdAppGetRegistrationCommandResultData> GetOrCreateRegistration(HerdAppGetOrCreateRegistrationCommand getOrCreateRegistrationCommand)
        {
            return ProcessCommand<HerdAppGetRegistrationCommandResultData>(result =>
            {
                result.Data = new HerdAppGetRegistrationCommandResultData
                {
                    Registration = _data.GetAppRegistration(getOrCreateRegistrationCommand.Instance)
                        ?? _data.CreateAppRegistration(new MastodonApiWrapper(getOrCreateRegistrationCommand.Instance).RegisterApp().Synchronously())
                };
            });
        }

        #endregion App registration

        #region Users

        public HerdAppCommandResult<HerdAppGetUserCommandResultData> GetUser(HerdAppGetUserCommand getUserCommand)
        {
            return ProcessCommand<HerdAppGetUserCommandResultData>(result =>
            {
                result.Data = new HerdAppGetUserCommandResultData
                {
                    User = _data.GetUser(getUserCommand.UserID)
                };
            });
        }

        public HerdAppCommandResult<HerdAppCreateUserCommandResultData> CreateUser(HerdAppCreateUserCommand createUserCommand)
        {
            return ProcessCommand<HerdAppCreateUserCommandResultData>(result =>
            {
                var userByEmail = _data.GetUser(createUserCommand.Email);
                if (userByEmail != null)
                {
                    throw new HerdAppUserErrorException("That email address has already been taken");
                }

                var saltKey = _saltGenerator.Next();
                result.Data = new HerdAppCreateUserCommandResultData
                {
                    User = _data.CreateUser(new HerdUserAccountDataModel
                    {
                        Email = createUserCommand.Email,
                        Security = new HerdUserAccountSecurity
                        {
                            SaltKey = saltKey,
                            SaltedPassword = createUserCommand.PasswordPlainText.Hashed(saltKey)
                        }
                    })
                };
                result.Data.Profile = _data.CreateProfile(new HerdUserProfileDataModel
                {
                    FirstName = createUserCommand.FirstName,
                    LastName = createUserCommand.LastName,
                    UserID = result.Data.User.ID
                });
                result.Data.User.ProfileID = result.Data.Profile.ID;
                _data.UpdateUser(result.Data.User);
            });
        }

        public HerdAppCommandResult<HerdAppLoginUserCommandResultData> LoginUser(HerdAppLoginUserCommand loginUserCommand)
        {
            return ProcessCommand<HerdAppLoginUserCommandResultData>(result =>
            {
                var userByEmail = _data.GetUser(loginUserCommand.Email);
                if (userByEmail?.PasswordIs(loginUserCommand.PasswordPlainText) != true)
                {
                    throw new HerdAppUserErrorException("Wrong email or password");
                }
                result.Data = new HerdAppLoginUserCommandResultData
                {
                    User = userByEmail
                };
            });
        }

        public HerdAppCommandResult UpdateUserMastodonConnection(HerdAppUpdateUserMastodonConnectionCommand updateUserMastodonConnectionCommand)
        {
            return ProcessCommand(result =>
            {
                // Check the token
                if (string.IsNullOrWhiteSpace(updateUserMastodonConnectionCommand.Token))
                {
                    throw new HerdAppUserErrorException("Token cannot be empty");
                }

                // Check the user ID
                var user = _data.GetUser(updateUserMastodonConnectionCommand.UserID);
                if (user == null)
                {
                    throw new HerdAppUserErrorException("Could not find a user with that ID");
                }

                // Connect with the one-time use code to get the permanent code
                _mastodonApiWrapper.AppRegistration = _data.GetAppRegistration(updateUserMastodonConnectionCommand.AppRegistrationID)
                    ?? throw new HerdAppUserErrorException("Could not find a registration with that ID");
                _mastodonApiWrapper.UserMastodonConnectionDetails = _mastodonApiWrapper.Connect(updateUserMastodonConnectionCommand.Token).Synchronously();

                // Update the user details
                user.MastodonConnection = _mastodonApiWrapper.UserMastodonConnectionDetails;
                _data.UpdateUser(user);
            });
        }

        #endregion Users

        #region Feed

        public HerdAppCommandResult<HerdAppGetRecentFeedItemsCommandResultData> GetRecentFeedItems(HerdAppGetRecentFeedItemsCommand getRecentFeedItemsCommand)
        {
            return ProcessCommand<HerdAppGetRecentFeedItemsCommandResultData>(result =>
            {
                result.Data = new HerdAppGetRecentFeedItemsCommandResultData
                {
                    RecentFeedItems = _mastodonApiWrapper.GetRecentStatuses(getRecentFeedItemsCommand.MaxCount).Synchronously().Select(s => new Status
                    {
                        Account = new Account
                        {
                            AccountName = s.Account.AccountName,
                            AvatarUrl = s.Account.AvatarUrl,
                            DisplayName = s.Account.DisplayName,
                            Id = s.Account.Id,
                            ProfileUrl = s.Account.ProfileUrl,
                            UserName = s.Account.UserName,
                        },
                        Content = s.Content,
                        CreatedAt = s.CreatedAt,
                        Favourited = s.Favourited,
                        FavouritesCount = s.FavouritesCount,
                        Id = s.Id,
                        InReplyToAccountId = s.InReplyToAccountId,
                        InReplyToId = s.InReplyToId,
                        MediaAttachments = s.MediaAttachments,
                        Mentions = s.Mentions,
                        Reblog = s.Reblog,
                        ReblogCount = s.ReblogCount,
                        Reblogged = s.Reblogged,
                        Sensitive = s.Sensitive,
                        SpoilerText = s.SpoilerText,
                        Tags = s.Tags,
                        Uri = s.Uri,
                        Url = s.Url,
                        Visibility = s.Visibility,
                    }).ToList()
                };
            });
        }

        public HerdAppCommandResult<HerdAppGetStatusCommandResultData> GetStatus(HerdAppGetStatusCommand getStatusCommand)
        {
            return ProcessCommand<HerdAppGetStatusCommandResultData>(result =>
            {
                var status = _mastodonApiWrapper.GetStatus(getStatusCommand.StatusId).Synchronously();
                var context = _mastodonApiWrapper.GetStatusContext(getStatusCommand.StatusId).Synchronously();
                result.Data = new HerdAppGetStatusCommandResultData
                {
                    Status = new Status
                    {
                        Account = new Account
                        {
                            AccountName = status.Account.AccountName,
                            AvatarUrl = status.Account.AvatarUrl,
                            DisplayName = status.Account.DisplayName,
                            Id = status.Account.Id,
                            ProfileUrl = status.Account.ProfileUrl,
                            UserName = status.Account.UserName,
                        },
                        Content = status.Content,
                        CreatedAt = status.CreatedAt,
                        Favourited = status.Favourited,
                        FavouritesCount = status.FavouritesCount,
                        Id = status.Id,
                        InReplyToAccountId = status.InReplyToAccountId,
                        InReplyToId = status.InReplyToId,
                        MediaAttachments = status.MediaAttachments,
                        Mentions = status.Mentions,
                        Reblog = status.Reblog,
                        ReblogCount = status.ReblogCount,
                        Reblogged = status.Reblogged,
                        Sensitive = status.Sensitive,
                        SpoilerText = status.SpoilerText,
                        Tags = status.Tags,
                        Uri = status.Uri,
                        Url = status.Url,
                        Visibility = status.Visibility,
                    },
                    StatusContext = new StatusContext
                    {
                        Ancestors = context.Ancestors.Select(ancestor => new Status
                        {
                            Account = new Account
                            {
                                AccountName = ancestor.Account.AccountName,
                                AvatarUrl = ancestor.Account.AvatarUrl,
                                DisplayName = ancestor.Account.DisplayName,
                                Id = ancestor.Account.Id,
                                ProfileUrl = ancestor.Account.ProfileUrl,
                                UserName = ancestor.Account.UserName,
                            },
                            Content = ancestor.Content,
                            CreatedAt = ancestor.CreatedAt,
                            Favourited = ancestor.Favourited,
                            FavouritesCount = ancestor.FavouritesCount,
                            Id = ancestor.Id,
                            InReplyToAccountId = ancestor.InReplyToAccountId,
                            InReplyToId = ancestor.InReplyToId,
                            MediaAttachments = ancestor.MediaAttachments,
                            Mentions = ancestor.Mentions,
                            Reblog = ancestor.Reblog,
                            ReblogCount = ancestor.ReblogCount,
                            Reblogged = ancestor.Reblogged,
                            Sensitive = ancestor.Sensitive,
                            SpoilerText = ancestor.SpoilerText,
                            Tags = ancestor.Tags,
                            Uri = ancestor.Uri,
                            Url = ancestor.Url,
                            Visibility = ancestor.Visibility,
                        }).ToList(),
                        Descendants = context.Descendants.Select(descendant => new Status
                        {
                            Account = new Account
                            {
                                AccountName = descendant.Account.AccountName,
                                AvatarUrl = descendant.Account.AvatarUrl,
                                DisplayName = descendant.Account.DisplayName,
                                Id = descendant.Account.Id,
                                ProfileUrl = descendant.Account.ProfileUrl,
                                UserName = descendant.Account.UserName,
                            },
                            Content = descendant.Content,
                            CreatedAt = descendant.CreatedAt,
                            Favourited = descendant.Favourited,
                            FavouritesCount = descendant.FavouritesCount,
                            Id = descendant.Id,
                            InReplyToAccountId = descendant.InReplyToAccountId,
                            InReplyToId = descendant.InReplyToId,
                            MediaAttachments = descendant.MediaAttachments,
                            Mentions = descendant.Mentions,
                            Reblog = descendant.Reblog,
                            ReblogCount = descendant.ReblogCount,
                            Reblogged = descendant.Reblogged,
                            Sensitive = descendant.Sensitive,
                            SpoilerText = descendant.SpoilerText,
                            Tags = descendant.Tags,
                            Uri = descendant.Uri,
                            Url = descendant.Url,
                            Visibility = descendant.Visibility,
                        }).ToList(),
                    }
                };
            });
        }

        public HerdAppCommandResult CreateNewPost(HerdAppCreateNewPostCommand createNewPostCommand)
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

        #region Private helpers

        private HerdAppCommandResult<D> ProcessCommand<D>(Action<HerdAppCommandResult<D>> doWork)
            where D : HerdAppCommandResultData
        {
            return ProcessCommand(new HerdAppCommandResult<D>(), doWork);
        }

        private HerdAppCommandResult ProcessCommand(Action<HerdAppCommandResult> doWork)
        {
            return ProcessCommand(new HerdAppCommandResult(), doWork);
        }

        private R ProcessCommand<R>(R result, Action<R> doWork) where R : HerdAppCommandResult
        {
            try
            {
                doWork(result);
            }
            catch (Exception e)
            {
                var errorID = Guid.NewGuid();
                _logger.Error(errorID, "Error in HerdApp", null, e);
                result.Errors.Add((e as HerdAppErrorException)?.Error ?? new HerdAppSystemError
                {
                    Message = $"Unhandled exception: {e.Message}"
                });
            }
            return result;
        }

        #endregion Private helpers
    }
}
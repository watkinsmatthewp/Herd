﻿using Herd.Business.App.Exceptions;
using Herd.Business.Models;
using Herd.Business.Models.Errors;
using Herd.Data.Providers;
using System;
using System.Linq;
using Herd.Business.Models.CommandResultData;
using Herd.Business.Models.Commands;
using Herd.Data.Models;
using System.Threading.Tasks;
using Herd.Core;
using System.Collections.Generic;
using static Herd.Business.Models.CommandResultData.HerdAppGetRecentFeedItemsCommandResultData;
using Herd.Logging;

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

        #endregion

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

        #endregion

        #region Feed

        public HerdAppCommandResult<HerdAppGetRecentFeedItemsCommandResultData> GetRecentFeedItems(HerdAppGetRecentFeedItemsCommand getRecentFeedItemsCommand)
        {
            return ProcessCommand<HerdAppGetRecentFeedItemsCommandResultData>(result =>
            {
                result.Data = new HerdAppGetRecentFeedItemsCommandResultData
                {
                    RecentFeedItems = _mastodonApiWrapper.GetRecentStatuses(getRecentFeedItemsCommand.MaxCount).Synchronously().Select(s => new RecentFeedItem
                    {
                        Account = new Models.MastodonWrappers.Account {
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

                // Test data
                if (result.Data.RecentFeedItems.Count <= 25)
                {
                    result.Data.RecentFeedItems.Add(new RecentFeedItem
                    {
                        Account = new Models.MastodonWrappers.Account
                        {
                           DisplayName = "Thomas Ortiz",
                           UserName = "tdortiz",
                           AvatarUrl = "https://i.ytimg.com/vi/mRSTCUTtjWc/hqdefault.jpg",
                        },
                        Content = "The best thing about a boolean is even if you are wrong, you are only off by a bit."
                    });
                    result.Data.RecentFeedItems.Add(new RecentFeedItem
                    {
                        Account = new Models.MastodonWrappers.Account
                        {
                            DisplayName = "Matthew Watkins",
                            UserName = "mpwatki2",
                            AvatarUrl = "https://calculatedbravery.files.wordpress.com/2014/01/nerd.jpg",
                        },
                        Content = "Always code as if the person who ends up maintaining your code will be a violent psychopath who knows where you live."
                    });
                    result.Data.RecentFeedItems.Add(new RecentFeedItem
                    {
                        Account = new Models.MastodonWrappers.Account
                        {
                            DisplayName = "Jacob Stone",
                            UserName = "jcstone3",
                            AvatarUrl = "http://mist.motifake.com/image/demotivational-poster/1003/pity-the-fool-mister-e-t-demotivational-poster-1267758828.jpg",
                        },
                        Content = "Programming today is a race between software engineers striving to build bigger " +
                                   "and better idiot-proof programs, and the universe trying to produce bigger and " +
                                   "better idiots. So far, the universe is winning."
                    });
                    result.Data.RecentFeedItems.Add(new RecentFeedItem
                    {
                        Account = new Models.MastodonWrappers.Account
                        {
                            DisplayName = "Dana Christo",
                            UserName = "dbchris3",
                            AvatarUrl = "https://yt3.ggpht.com/-AC_X27FHo80/AAAAAAAAAAI/AAAAAAAAAAA/YfGKh9RmAC0/s900-c-k-no-mo-rj-c0xffffff/photo.jpg",
                        },
                        Content = "I'm out."
                    });
                }
            });
        }

        #endregion

        #region Private helpers

        private HerdAppCommandResult<D> ProcessCommand<D>(Action<HerdAppCommandResult<D>> doWork)
            where D : HerdAppCommandResultData
        {
            return ProcessCommand(new HerdAppCommandResult<D>(), doWork);
        }

        private R ProcessCommand<R>(R result, Action<R> doWork)
            where R : HerdAppCommandResult
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
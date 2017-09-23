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

namespace Herd.Business
{
    public class HerdApp : IHerdApp
    {
        private const string NON_REDIRECT_URL = "urn:ietf:wg:oauth:2.0:oob";

        private static Random _saltGenerator = new Random(Guid.NewGuid().GetHashCode());

        public static IHerdApp Instance { get; } = new HerdApp();

        public IHerdDataProvider Data { get; } = new HerdFileDataProvider();

        private HerdApp()
        {
        }

        #region Users

        public HerdAppCommandResult<HerdAppGetUserCommandResultData> GetUser(HerdAppGetUserCommand getUserCommand)
        {
            return ProcessCommand<HerdAppGetUserCommandResultData>(result =>
            {
                result.Data = new HerdAppGetUserCommandResultData
                {
                    User = Data.GetUser(getUserCommand.UserID)
                };
            });
        }

        public HerdAppCommandResult<HerdAppCreateUserCommandResultData> CreateUser(HerdAppCreateUserCommand createUserCommand)
        {
            return ProcessCommand<HerdAppCreateUserCommandResultData>(result =>
            {
                var userByEmail = Data.GetUser(createUserCommand.Email);
                if (userByEmail != null)
                {
                    throw new HerdAppUserErrorException("That email address has already been taken");
                }

                var saltKey = _saltGenerator.Next();
                result.Data = new HerdAppCreateUserCommandResultData
                {
                    User = Data.CreateUser(new HerdUserAccountDataModel
                    {
                        Email = createUserCommand.Email,
                        Security = new HerdUserAccountSecurity
                        {
                            SaltKey = saltKey,
                            SaltedPassword = createUserCommand.PasswordPlainText.Hashed(saltKey)
                        }
                    })
                };
                result.Data.Profile = Data.CreateProfile(new HerdUserProfileDataModel
                {
                    FirstName = createUserCommand.FirstName,
                    LastName = createUserCommand.LastName,
                    UserID = result.Data.User.ID
                });
                result.Data.User.ProfileID = result.Data.Profile.ID;
                Data.UpdateUser(result.Data.User);
            });
        }

        public HerdAppCommandResult<HerdAppLoginUserCommandResultData> LoginUser(HerdAppLoginUserCommand loginUserCommand)
        {
            return ProcessCommand<HerdAppLoginUserCommandResultData>(result =>
            {
                var userByEmail = Data.GetUser(loginUserCommand.Email);
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

        #region App registration

        public HerdAppCommandResult<HerdAppGetRegistrationCommandResultData> GetRegistration(HerdAppGetRegistrationCommand getRegistrationCommand)
        {
            return ProcessCommand<HerdAppGetRegistrationCommandResultData>(result =>
            {
                result.Data = new HerdAppGetRegistrationCommandResultData
                {
                    Registration = Data.GetAppRegistration(getRegistrationCommand.ID)
                };
            });
        }

        public HerdAppCommandResult<HerdAppGetOAuthURLCommandResultData> GetOAuthURL(HerdAppGetOAuthURLCommand getOAuthUrlCommand)
        {
            return ProcessCommand<HerdAppGetOAuthURLCommandResultData>(result =>
            {
                var returnURL = string.IsNullOrWhiteSpace(getOAuthUrlCommand.ReturnURL) ? NON_REDIRECT_URL : getOAuthUrlCommand.ReturnURL;

                getOAuthUrlCommand.ApiWrapper.AppRegistration =
                    Data.GetAppRegistration(getOAuthUrlCommand.AppRegistrationID) ?? throw new HerdAppUserErrorException("No app registration with that ID");

                result.Data = new HerdAppGetOAuthURLCommandResultData
                {
                    URL = getOAuthUrlCommand.ApiWrapper.GetOAuthUrl(getOAuthUrlCommand.ReturnURL)
                };
            });
        }

        public HerdAppCommandResult<HerdAppGetRegistrationCommandResultData> GetOrCreateRegistration(HerdAppGetOrCreateRegistrationCommand getOrCreateRegistrationCommand)
        {
            return ProcessCommand<HerdAppGetRegistrationCommandResultData>(result =>
            {
                result.Data = new HerdAppGetRegistrationCommandResultData
                {
                    Registration = Data.GetAppRegistration(getOrCreateRegistrationCommand.Instance)
                        ?? Data.CreateAppRegistration(new MastodonApiWrapper(getOrCreateRegistrationCommand.Instance).RegisterApp().Synchronously())
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
                    RecentFeedItems = getRecentFeedItemsCommand.MastodonApiWrapper
                        .GetRecentStatuses(getRecentFeedItemsCommand.MaxCount).Synchronously()
                        .Select(s => new RecentFeedItem
                        {
                            Text = s.SpoilerText,
                            AuthorUserName = s.Account.UserName,
                            AuthorDisplayname = s.Account.DisplayName,
                            AuthorAvatarURL = s.Account.AvatarUrl
                        }).ToList()
                };

                if (result.Data.RecentFeedItems.Count == 0)
                {
                    result.Data.RecentFeedItems = new List<RecentFeedItem>
                    {
                        new RecentFeedItem
                        {
                            AuthorDisplayname = "Thomas Ortiz",
                            AuthorUserName = "tdortiz",
                            AuthorAvatarURL = "https://i.ytimg.com/vi/mRSTCUTtjWc/hqdefault.jpg",
                            Text = "The best thing about a boolean is even if you are wrong, you are only off by a bit."
                        },
                        new RecentFeedItem
                        {
                            AuthorDisplayname = "Matthew Watkins",
                            AuthorUserName = "mpwatki2",
                            AuthorAvatarURL = "https://calculatedbravery.files.wordpress.com/2014/01/nerd.jpg",
                            Text = "Always code as if the person who ends up maintaining your code will be a violent psychopath who knows where you live."
                        },
                        new RecentFeedItem
                        {
                            AuthorDisplayname = "Jacob Stone",
                            AuthorUserName = "jcstone3",
                            AuthorAvatarURL = "http://mist.motifake.com/image/demotivational-poster/1003/pity-the-fool-mister-e-t-demotivational-poster-1267758828.jpg",
                            Text = "Programming today is a race between software engineers striving to build bigger and better idiot-proof programs, and the universe trying to produce bigger and better idiots. So far, the universe is winning."
                        }
                    };
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
            catch (HerdAppErrorException e)
            {
                result.Errors.Add(e.Error);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result.Errors.Add(new HerdAppSystemError
                {
                    Message = $"Unhandled exception: {e.Message}"
                });
            }
            return result;
        }

        #endregion Private helpers
    }
}
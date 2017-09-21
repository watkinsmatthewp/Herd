using Herd.Business.App.Exceptions;
using Herd.Business.Models;
using Herd.Business.Models.Errors;
using Herd.Data.Providers;
using System;
using Herd.Business.Models.CommandResultData;
using Herd.Business.Models.Commands;
using Herd.Data.Models;

namespace Herd.Business
{
    public class HerdApp : IHerdApp
    {
        private static Random _saltGenerator = new Random(Guid.NewGuid().GetHashCode());

        public static HerdApp Instance { get; } = new HerdApp();

        public IHerdDataProvider Data { get; } = new HerdFileDataProvider();

        private HerdApp()
        {
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
                result.Data.User = Data.CreateUser(new HerdUserDataModel
                {
                    Email = createUserCommand.Email,
                    FirstName = createUserCommand.FirstName,
                    LastName = createUserCommand.LastName,
                    SaltKey = saltKey,
                    SaltedPassword = createUserCommand.PasswordPlainText.Hashed(saltKey)
                });
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
                result.Data.User = userByEmail;
            });
        }

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
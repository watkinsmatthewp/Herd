using Herd.Business.App.Exceptions;
using Herd.Business.Models;
using Herd.Business.Models.Errors;
using Herd.Data.Providers;
using System;
using Herd.Business.Models.CommandResultData;
using Herd.Business.Models.Commands;

namespace Herd.Business
{
    public class HerdApp : IHerdApp
    {
        public static HerdApp Instance { get; } = new HerdApp();

        public IHerdDataProvider Data { get; } = new HerdFileDataProvider();

        private HerdApp()
        {
        }

        public HerdAppCommandResult<HerdAppCreateUserCommandResultData> CreateUser(HerdAppCreateUserCommand createUserCommand)
        {
            return ProcessCommand<HerdAppCreateUserCommandResultData>(result =>
            {
                throw new NotImplementedException();
            });
        }

        public HerdAppCommandResult<HerdAppLoginUserCommandResultData> LoginUser(HerdAppLoginUserCommand loginUserCommand)
        {
            return ProcessCommand<HerdAppLoginUserCommandResultData>(result =>
            {
                throw new NotImplementedException();
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
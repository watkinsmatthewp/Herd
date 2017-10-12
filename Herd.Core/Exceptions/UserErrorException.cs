using Herd.Core.Errors;
using System;

namespace Herd.Core.Exceptions
{
    public class UserErrorException : ErrorException<UserError>
    {
        public const string ERROR_TYPE = "User";

        protected UserErrorException(string message)
            : base(new UserError(), message)
        {
        }

        protected UserErrorException(string message, Exception innerException)
            : base(new UserError(), message, innerException)
        {
        }

        protected override string ErrorType => ERROR_TYPE;
    }
}
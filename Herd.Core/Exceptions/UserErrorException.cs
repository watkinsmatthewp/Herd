using Herd.Core.Errors;
using System;

namespace Herd.Core.Exceptions
{
    public class UserErrorException : ErrorException<UserError>
    {
        public UserErrorException(string message)
            : base(new UserError(), message)
        {
        }

        public UserErrorException(string message, Exception innerException)
            : base(new UserError(), message, innerException)
        {
        }
    }
}
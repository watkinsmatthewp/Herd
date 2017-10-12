using Herd.Core.Errors;
using System;

namespace Herd.Core.Exceptions
{
    public class SystemErrorException : ErrorException<SystemError>
    {
        public const string ERROR_TYPE = "System";

        protected SystemErrorException(string message)
            : base(new SystemError(), message)
        {
        }

        protected SystemErrorException(string message, Exception innerException)
            : base(new SystemError(), message, innerException)
        {
        }

        protected override string ErrorType => ERROR_TYPE;
    }
}
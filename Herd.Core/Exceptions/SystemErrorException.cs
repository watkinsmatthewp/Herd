using Herd.Core.Errors;
using System;

namespace Herd.Core.Exceptions
{
    public class SystemErrorException : ErrorException<SystemError>
    {
        public SystemErrorException(string message)
            : base(new SystemError(), message)
        {
        }

        public SystemErrorException(string message, Exception innerException)
            : base(new SystemError(), message, innerException)
        {
        }
    }
}
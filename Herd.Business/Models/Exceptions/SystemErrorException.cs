using Herd.Business.Models.Errors;
using System;

namespace Herd.Business.App.Exceptions
{
    public class SystemErrorException : ErrorException<SystemError>
    {
        protected SystemErrorException()
            : base()
        {
        }

        protected SystemErrorException(string message)
            : base(message)
        {
        }

        protected SystemErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public override SystemError SpecificError => new SystemError
        {
            Message = Message
        };
    }
}
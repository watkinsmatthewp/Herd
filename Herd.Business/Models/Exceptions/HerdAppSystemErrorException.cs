using Herd.Business.Models.Errors;
using System;

namespace Herd.Business.App.Exceptions
{
    public class HerdAppSystemErrorException : HerdAppErrorException<SystemError>
    {
        protected HerdAppSystemErrorException()
            : base()
        {
        }

        protected HerdAppSystemErrorException(string message)
            : base(message)
        {
        }

        protected HerdAppSystemErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public override SystemError SpecificError => new SystemError
        {
            Message = Message
        };
    }
}
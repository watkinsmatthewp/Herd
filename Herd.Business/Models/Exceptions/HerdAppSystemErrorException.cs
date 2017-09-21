using Herd.Business.Models.Errors;
using System;

namespace Herd.Business.App.Exceptions
{
    public class HerdAppSystemErrorException : HerdAppErrorException<HerdAppSystemError>
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

        public override HerdAppSystemError SpecificError => new HerdAppSystemError
        {
            Message = Message
        };
    }
}
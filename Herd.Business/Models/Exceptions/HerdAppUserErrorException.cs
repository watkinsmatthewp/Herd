using Herd.Business.Models.Errors;
using System;

namespace Herd.Business.App.Exceptions
{
    public class HerdAppUserErrorException : HerdAppErrorException<UserError>
    {
        public HerdAppUserErrorException()
        {
        }

        public HerdAppUserErrorException(string message)
            : base(message)
        {
        }

        public HerdAppUserErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public override UserError SpecificError => new UserError
        {
            Message = Message
        };
    }
}
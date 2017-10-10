using Herd.Business.Models.Errors;
using System;

namespace Herd.Business.App.Exceptions
{
    public class UserErrorException : ErrorException<UserError>
    {
        public UserErrorException()
        {
        }

        public UserErrorException(string message)
            : base(message)
        {
        }

        public UserErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public override UserError SpecificError => new UserError
        {
            Message = Message
        };
    }
}
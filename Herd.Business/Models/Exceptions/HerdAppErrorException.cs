using Herd.Business.Models.Errors;
using System;

namespace Herd.Business.App.Exceptions
{
    public abstract class HerdAppErrorException : Exception
    {
        protected HerdAppErrorException()
            : base()
        {
        }

        protected HerdAppErrorException(string message)
            : base(message)
        {
        }

        protected HerdAppErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public abstract HerdAppError Error { get; }
    }

    public abstract class HerdAppErrorException<T> : HerdAppErrorException where T : HerdAppError
    {
        public override HerdAppError Error => SpecificError;
        public abstract T SpecificError { get; }

        protected HerdAppErrorException()
            : base()
        {
        }

        protected HerdAppErrorException(string message)
            : base(message)
        {
        }

        protected HerdAppErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
using Herd.Business.Models.Errors;
using System;

namespace Herd.Business.App.Exceptions
{
    public abstract class ErrorException : Exception
    {
        protected ErrorException()
            : base()
        {
        }

        protected ErrorException(string message)
            : base(message)
        {
        }

        protected ErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public abstract Error Error { get; }
    }

    public abstract class ErrorException<T> : ErrorException where T : Error
    {
        public override Error Error => SpecificError;
        public abstract T SpecificError { get; }

        protected ErrorException()
            : base()
        {
        }

        protected ErrorException(string message)
            : base(message)
        {
        }

        protected ErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
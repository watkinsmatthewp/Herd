using Herd.Core.Errors;
using System;

namespace Herd.Core.Exceptions
{
    public abstract class ErrorException : Exception
    {
        protected static readonly Random ERROR_ID_GENERATOR = new Random(Guid.NewGuid().GetHashCode());

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
        readonly T _error;

        public override Error Error => _error;

        protected ErrorException(T error, string message)
            : base(message)
        {
            _error = BuildError(error);
        }

        protected ErrorException(T error, string message, Exception innerException)
            : base(message, innerException)
        {
            _error = BuildError(error);
        }

        T BuildError(T error)
        {
            error.Id = ERROR_ID_GENERATOR.Next();
            error.Message = Message;
            return error;
        }
    }
}